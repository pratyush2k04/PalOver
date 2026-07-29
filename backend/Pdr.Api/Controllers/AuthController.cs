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

        Response.Cookies.Append("pdr_jwt", result.Token!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
        });

        return Ok(new { message = "Logged in successfully." });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("pdr_jwt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { message = "Logged out successfully." });
    }
}
