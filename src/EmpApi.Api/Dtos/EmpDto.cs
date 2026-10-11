using System.ComponentModel.DataAnnotations;

namespace EmpApi.Dtos;

public class EmpRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Position { get; set; }

    [Range(0, 100000000)]
    public decimal Salary { get; set; }

    [Required]
    public DateOnly? JoinDate { get; set; }
}

public record EmpResponse(int Id, string Name, string Email, string Department, string? Position, decimal Salary, DateOnly JoinDate);

public class LoginRequest
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public record LoginResponse(string Token, DateTime ExpiresAtUtc);
