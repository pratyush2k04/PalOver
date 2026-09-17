using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages access items and risk evaluation for engagement technical readiness.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/access-items")]
[RequireEngagementAccess]
[Produces("application/json")]
public class AccessTrackerController(IAccessTrackerService accessTrackerService) : ControllerBase
{
    /// <summary>
    /// Retrieves all access items for an engagement.
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <returns>A list of access items.</returns>
    [HttpGet]
    [EndpointSummary("Get all access items for an engagement")]
    [ProducesResponseType(typeof(List<AccessItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccessItems(int engagementId)
    {
        var items = await accessTrackerService.GetAccessItemsAsync(engagementId);
        return Ok(items);
    }

    /// <summary>
    /// Adds a custom access item to an engagement (Tech Lead only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="request">The custom access item request payload.</param>
    /// <returns>The created access item.</returns>
    [HttpPost]
    [RequireEngagementRole(EngagementRoleEnum.TechLead)]
    [EndpointSummary("Add custom access item (Tech Lead only)")]
    [ProducesResponseType(typeof(AccessItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCustomAccessItem(int engagementId, [FromBody] AddCustomAccessItemRequest request)
    {
        var item = await accessTrackerService.AddCustomAccessItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetAccessItems), new { engagementId }, item);
    }

    /// <summary>
    /// Manually triggers client-side risk evaluation rules for an engagement (Tech Lead only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <returns>Confirmation message on completing risk evaluation.</returns>
    [HttpPost("evaluate-risks")]
    [RequireEngagementRole(EngagementRoleEnum.TechLead)]
    [EndpointSummary("Evaluate client-side risks (Tech Lead only)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EvaluateClientSideRisks(int engagementId)
    {
        await accessTrackerService.EvaluateClientSideRisksAsync(engagementId);
        return Ok(new { message = AccessTrackerConstants.Messages.RiskEvaluationCompleted });
    }
}

/// <summary>
/// Handles item-level operations for individual access items.
/// </summary>
[ApiController]
[Authorize]
[Route("api/access-items")]
[Produces("application/json")]
public class AccessItemsController(IAccessTrackerService accessTrackerService) : ControllerBase
{
    /// <summary>
    /// Updates the status of an access item (Tech Lead only).
    /// </summary>
    /// <param name="id">The access item ID.</param>
    /// <param name="request">The status update payload.</param>
    /// <returns>The updated access item.</returns>
    [HttpPatch("{id:int}/status")]
    [RequireItemEngagementRole(ItemEntityType.AccessItem, EngagementRoleEnum.TechLead)]
    [EndpointSummary("Update access item status (Tech Lead only)")]
    [ProducesResponseType(typeof(AccessItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAccessItemStatusRequest request)
    {
        var item = await accessTrackerService.UpdateAccessItemStatusAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Updates the assigned owner of an access item (Tech Lead only).
    /// </summary>
    /// <param name="id">The access item ID.</param>
    /// <param name="request">The owner update payload.</param>
    /// <returns>The updated access item.</returns>
    [HttpPatch("{id:int}/owner")]
    [RequireItemEngagementRole(ItemEntityType.AccessItem, EngagementRoleEnum.TechLead)]
    [EndpointSummary("Update access item owner (Tech Lead only)")]
    [ProducesResponseType(typeof(AccessItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] UpdateAccessItemOwnerRequest request)
    {
        var item = await accessTrackerService.UpdateAccessItemOwnerAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Soft-deletes an access item (Tech Lead only).
    /// </summary>
    /// <param name="id">The access item ID.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("{id:int}")]
    [RequireItemEngagementRole(ItemEntityType.AccessItem, EngagementRoleEnum.TechLead)]
    [EndpointSummary("Delete access item (Tech Lead only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccessItem(int id)
    {
        await accessTrackerService.DeleteAccessItemAsync(id);
        return NoContent();
    }
}
