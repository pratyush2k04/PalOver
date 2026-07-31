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

        var responseDto = new LoginResponseDto
        {
            Token = result.Token!
        };

        return Ok(responseDto);
    }
}
