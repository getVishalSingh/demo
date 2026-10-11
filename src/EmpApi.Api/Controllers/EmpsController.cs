using EmpApi.Data;
using EmpApi.Dtos;
using EmpApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmpApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EmpsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpResponse>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var items = await db.Emps.AsNoTracking().OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(items.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmpResponse>> Get(int id, CancellationToken ct)
    {
        var e = await db.Emps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? NotFound() : Ok(ToResponse(e));
    }

    [HttpPost]
    public async Task<ActionResult<EmpResponse>> Create(EmpRequest request, CancellationToken ct)
    {
        if (await db.Emps.AnyAsync(x => x.Email == request.Email, ct))
            return Conflict(new ProblemDetails { Title = "Email already exists", Status = 409 });
        var e = new Emp();
        Apply(e, request);
        db.Emps.Add(e);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToResponse(e));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmpResponse>> Update(int id, EmpRequest request, CancellationToken ct)
    {
        var e = await db.Emps.FindAsync([id], ct);
        if (e is null) return NotFound();
        if (await db.Emps.AnyAsync(x => x.Email == request.Email && x.Id != id, ct))
            return Conflict(new ProblemDetails { Title = "Email already exists", Status = 409 });
        Apply(e, request);
        await db.SaveChangesAsync(ct);
        return Ok(ToResponse(e));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var e = await db.Emps.FindAsync([id], ct);
        if (e is null) return NotFound();
        db.Emps.Remove(e);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(Emp e, EmpRequest r)
    {
        e.Name = r.Name; e.Email = r.Email; e.Department = r.Department;
        e.Position = r.Position; e.Salary = r.Salary; e.JoinDate = r.JoinDate!.Value;
    }

    internal static EmpResponse ToResponse(Emp e) =>
        new(e.Id, e.Name, e.Email, e.Department, e.Position, e.Salary, e.JoinDate);
}
