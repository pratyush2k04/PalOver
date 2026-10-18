using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.Constants;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides system role management and user assignment queries.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RolesController(IRoleRepository roleRepository) : ControllerBase
{
    /// <summary>
    /// Retrieves all system roles (Admin only).
    /// </summary>
    /// <returns>A list of system roles.</returns>
    [HttpGet]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Get all system roles (Admin only)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await roleRepository.GetAllAsync();
        return Ok(roles.Select(r => new { r.Id, r.Name }));
    }

    /// <summary>
    /// Retrieves all users assigned to a specific role for directory lookup.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <returns>A list of users assigned to the specified role.</returns>
    [HttpGet("{id:int}/users")]
    [Authorize]
    [EndpointSummary("Get all users assigned to a role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsersInRole(int id)
    {
        var users = await roleRepository.GetUsersInRoleAsync(id);
        return Ok(users.Select(u => new { u.Id, u.FullName, u.Email, u.RoleId, RoleName = u.Role.Name }));
    }
}
