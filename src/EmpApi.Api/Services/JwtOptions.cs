namespace EmpApi.Services;

public class JwtOptions
{
    public string Issuer { get; set; } = "EmpApi";
    public string Audience { get; set; } = "EmpApi";
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

public class AdminUserOptions
{
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = string.Empty;
}
