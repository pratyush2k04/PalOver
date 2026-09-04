using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.EnvironmentTooling;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages environment and tooling readiness items for an engagement.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/environment-tooling")]
[Produces("application/json")]
public class EnvironmentToolingController(IEnvironmentToolingService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all environment and tooling items for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>A list of environment and tooling items.</returns>
    [HttpGet]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    [EndpointSummary("Get environment & tooling items")]
    [ProducesResponseType(typeof(EnvironmentToolingReadinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItems(int engagementId)
    {
        var response = await service.GetItemsByEngagementIdAsync(engagementId);
        return Ok(response);
    }

    /// <summary>
    /// Adds a custom environment or tooling item to an engagement (Tech Lead only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The custom item request payload.</param>
    /// <returns>The created environment/tooling item.</returns>
    [HttpPost]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Add custom environment item (Tech Lead only)")]
    [ProducesResponseType(typeof(EnvironmentToolingItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCustomItem(int engagementId, [FromBody] AddCustomEnvironmentToolingItemRequest request)
    {
        var item = await service.AddCustomItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetItems), new { engagementId }, item);
    }
}

/// <summary>
/// Handles item-level operations for individual environment and tooling items.
/// </summary>
[ApiController]
[Authorize]
[Route("api/environment-tooling")]
[Produces("application/json")]
public class EnvironmentToolingItemsController(IEnvironmentToolingService service) : ControllerBase
{
    /// <summary>
    /// Updates the status of an environment or tooling item (Tech Lead only).
    /// </summary>
    /// <param name="id">The item ID.</param>
    /// <param name="request">The status update payload.</param>
    /// <returns>The updated environment/tooling item.</returns>
    [HttpPatch("{id:int}/status")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Update environment item status (Tech Lead only)")]
    [ProducesResponseType(typeof(EnvironmentToolingItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateEnvironmentToolingStatusRequest request)
    {
        var item = await service.UpdateStatusAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Updates the assigned owner of an environment or tooling item (Tech Lead only).
    /// </summary>
    /// <param name="id">The item ID.</param>
    /// <param name="request">The owner update payload.</param>
    /// <returns>The updated environment/tooling item.</returns>
    [HttpPatch("{id:int}/owner")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Update environment item owner (Tech Lead only)")]
    [ProducesResponseType(typeof(EnvironmentToolingItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] UpdateEnvironmentToolingOwnerRequest request)
    {
        var item = await service.UpdateOwnerAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Deletes an environment or tooling item (Tech Lead only).
    /// </summary>
    /// <param name="id">The item ID.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("{id:int}")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Delete environment item (Tech Lead only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await service.DeleteItemAsync(id);
        return NoContent();
    }
}
