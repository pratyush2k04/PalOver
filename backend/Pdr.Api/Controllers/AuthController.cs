using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Auth;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Handles authentication and login requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticates a user with email and password to return a JWT token.
    /// </summary>
    /// <param name="request">The login credentials (email and password).</param>
    /// <returns>A JWT authentication token upon successful login.</returns>
    [HttpPost("login")]
    [EndpointSummary("Authenticate user and return JWT token")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
