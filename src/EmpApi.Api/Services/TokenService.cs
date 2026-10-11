using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmpApi.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmpApi.Services;

public interface ITokenService
{
    LoginResponse? Authenticate(string username, string password);
}

public class TokenService(IOptions<JwtOptions> jwt, IOptions<AdminUserOptions> admin) : ITokenService
{
    public LoginResponse? Authenticate(string username, string password)
    {
        var a = admin.Value;
        if (string.IsNullOrEmpty(a.Password) || !FixedEquals(username, a.Username) || !FixedEquals(password, a.Password))
            return null;

        var o = jwt.Value;
        var expires = DateTime.UtcNow.AddMinutes(o.ExpiryMinutes);
        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(o.Issuer, o.Audience,
            [new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "Admin")],
            expires: expires, signingCredentials: creds);
        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    private static bool FixedEquals(string x, string y) =>
        CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(x)), SHA256.HashData(Encoding.UTF8.GetBytes(y)));
}
