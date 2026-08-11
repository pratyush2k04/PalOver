using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Users;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides user administration and role management (Admin only).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
[Produces("application/json")]
public class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    /// <summary>
    /// Retrieves paginated system users with optional search term filtering (Admin only).
    /// </summary>
    /// <param name="search">Optional search filter by full name or email.</param>
    /// <param name="page">Page index (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20).</param>
    /// <returns>A paginated list of system users.</returns>
    [HttpGet]
    [EndpointSummary("Get paginated users (Admin only)")]
    [ProducesResponseType(typeof(PagedResult<UserListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await userManagementService.GetUsersAsync(search, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Updates the assigned role of a system user (Admin only).
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">The request payload containing the new role ID.</param>
    /// <returns>The updated user details.</returns>
    [HttpPatch("{id:int}/role")]
    [EndpointSummary("Change user role (Admin only)")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleRequestDto request)
    {
        var user = await userManagementService.ChangeUserRoleAsync(id, request.NewRoleId);
        return Ok(user);
    }
}
