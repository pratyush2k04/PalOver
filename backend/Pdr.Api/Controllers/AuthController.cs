using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Auth;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new { message = result.ErrorMessage });
            }

            return Unauthorized(new { message = result.ErrorMessage });
        }

        var bearerToken = $"Bearer {result.Token}";

        Response.Headers["Authorization"] = bearerToken;
        Response.Headers["Access-Control-Expose-Headers"] = "Authorization";

        return Ok(new { message = "Logged in successfully." });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully." });
    }
}
