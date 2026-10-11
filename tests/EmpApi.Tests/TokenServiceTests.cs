using EmpApi.Services;
using Microsoft.Extensions.Options;

namespace EmpApi.Tests;

public class TokenServiceTests
{
    private static TokenService Create(string adminPassword = "pw") => new(
        Options.Create(new JwtOptions { Key = new string('k', 40) }),
        Options.Create(new AdminUserOptions { Username = "admin", Password = adminPassword }));

    [Fact] public void ValidCredentials_ReturnToken() => Assert.NotNull(Create().Authenticate("admin", "pw")?.Token);
    [Fact] public void WrongPassword_ReturnsNull() => Assert.Null(Create().Authenticate("admin", "bad"));
    [Fact] public void EmptyConfiguredPassword_AlwaysRejects() => Assert.Null(Create("").Authenticate("admin", ""));
}
