using EmpApi.Dtos;
using EmpApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmpApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var result = tokens.Authenticate(request.Username, request.Password);
        return result is null ? Unauthorized(new ProblemDetails { Title = "Invalid credentials", Status = 401 }) : Ok(result);
    }
}
