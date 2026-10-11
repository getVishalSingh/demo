namespace EmpApi.Models;

public class Emp
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? Position { get; set; }
    public decimal Salary { get; set; }
    public DateOnly JoinDate { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
