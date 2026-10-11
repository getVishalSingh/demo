using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmpApi.Dtos;

namespace EmpApi.Tests;

public class EmpsApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static EmpRequest Valid(string email = "a@x.com") => new()
    {
        Name = "Alice", Email = email, Department = "IT", Salary = 1000, JoinDate = new DateOnly(2024, 1, 1)
    };

    private async Task<HttpClient> AuthedAsync()
    {
        var c = factory.CreateClient();
        var r = await c.PostAsJsonAsync("/api/auth/login", new LoginRequest { Username = "admin", Password = "pass123" });
        r.EnsureSuccessStatusCode();
        var t = await r.Content.ReadFromJsonAsync<LoginResponse>();
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t!.Token);
        return c;
    }

    [Fact]
    public async Task Login_WithBadPassword_Returns401()
    {
        var r = await factory.CreateClient().PostAsJsonAsync("/api/auth/login", new LoginRequest { Username = "admin", Password = "x" });
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact]
    public async Task Emps_WithoutToken_Returns401()
    {
        var r = await factory.CreateClient().GetAsync("/api/emps");
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact]
    public async Task Crud_FullLifecycle()
    {
        var c = await AuthedAsync();
        var created = await c.PostAsJsonAsync("/api/emps", Valid("crud@x.com"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var emp = await created.Content.ReadFromJsonAsync<EmpResponse>();

        Assert.Equal(HttpStatusCode.OK, (await c.GetAsync($"/api/emps/{emp!.Id}")).StatusCode);

        var upd = Valid("crud@x.com"); upd.Name = "Bob";
        var put = await c.PutAsJsonAsync($"/api/emps/{emp.Id}", upd);
        Assert.Equal("Bob", (await put.Content.ReadFromJsonAsync<EmpResponse>())!.Name);

        Assert.Equal(HttpStatusCode.NoContent, (await c.DeleteAsync($"/api/emps/{emp.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await c.GetAsync($"/api/emps/{emp.Id}")).StatusCode);
    }

    [Fact]
    public async Task Create_InvalidEmail_Returns400()
    {
        var c = await AuthedAsync();
        var bad = Valid(); bad.Email = "nope";
        Assert.Equal(HttpStatusCode.BadRequest, (await c.PostAsJsonAsync("/api/emps", bad)).StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateEmail_Returns409()
    {
        var c = await AuthedAsync();
        await c.PostAsJsonAsync("/api/emps", Valid("dup@x.com"));
        Assert.Equal(HttpStatusCode.Conflict, (await c.PostAsJsonAsync("/api/emps", Valid("dup@x.com"))).StatusCode);
    }

    [Fact]
    public async Task Get_Unknown_Returns404()
    {
        var c = await AuthedAsync();
        Assert.Equal(HttpStatusCode.NotFound, (await c.GetAsync("/api/emps/9999")).StatusCode);
    }
}
