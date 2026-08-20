using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Profile;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides self-service access to the current user's own profile.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    /// <summary>
    /// Retrieves the current user's profile.
    /// </summary>
    /// <returns>The current user's profile details.</returns>
    [HttpGet]
    [EndpointSummary("Get current user's profile")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var result = await profileService.GetProfileAsync();
        return Ok(result);
    }

    /// <summary>
    /// Updates the current user's display name and contact email.
    /// </summary>
    /// <param name="request">The updated profile fields.</param>
    /// <returns>The updated profile details.</returns>
    [HttpPut]
    [EndpointSummary("Update current user's profile")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
    {
        var result = await profileService.UpdateProfileAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    /// <param name="request">The current and new password.</param>
    [HttpPost("change-password")]
    [EndpointSummary("Change current user's password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        await profileService.ChangePasswordAsync(request);
        return NoContent();
    }
}
