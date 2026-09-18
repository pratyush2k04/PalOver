using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.HandoverReadiness;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages Handover Readiness items, creation, and evidence tracking for an engagement.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/handover-readiness")]
[RequireEngagementAccess]
[Produces("application/json")]
public class HandoverReadinessController(IHandoverReadinessService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all Handover Readiness items for an engagement.
    /// Seeds default items if they do not exist yet.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>List of Handover Readiness items.</returns>
    [HttpGet]
    [EndpointSummary("Get handover readiness items")]
    [ProducesResponseType(typeof(List<HandoverReadinessItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HandoverReadinessItemDto>>> GetItems(int engagementId)
    {
        var items = await service.GetItemsAsync(engagementId);
        return Ok(items);
    }

    /// <summary>
    /// Adds a custom Handover Readiness item to an engagement under Handover area (Pre-Sales role required).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The custom item request.</param>
    /// <returns>The created Handover Readiness item.</returns>
    [HttpPost]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Add custom handover readiness item (Pre-Sales only)")]
    [ProducesResponseType(typeof(HandoverReadinessItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverReadinessItemDto>> AddCustomItem(int engagementId, [FromBody] AddCustomHandoverReadinessItemRequest request)
    {
        var item = await service.AddCustomItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetItems), new { engagementId }, item);
    }

    /// <summary>
    /// Attaches or updates an evidence document for a Handover Readiness item (Pre-Sales role required).
    /// </summary>
    [HttpPut("{itemId:int}/evidence")]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Attach or update handover readiness item evidence (Pre-Sales only)")]
    [ProducesResponseType(typeof(HandoverReadinessItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverReadinessItemDto>> SaveEvidence(int engagementId, int itemId, [FromBody] SaveHandoverReadinessEvidenceRequestDto request)
    {
        var item = await service.SaveItemEvidenceAsync(engagementId, itemId, request);
        return Ok(item);
    }
}

/// <summary>
/// Handles item-level operations for individual Handover Readiness items.
/// </summary>
[ApiController]
[Authorize]
[Route("api/handover-readiness")]
[Produces("application/json")]
public class HandoverReadinessItemsController(IHandoverReadinessService service) : ControllerBase
{
    /// <summary>
    /// Updates the status of a Handover Readiness item (Pre-Sales role required).
    /// Syncs risks if marked as Blocked or unblocked.
    /// </summary>
    /// <param name="id">The item ID.</param>
    /// <param name="request">The status update request.</param>
    /// <returns>The updated Handover Readiness item.</returns>
    [HttpPatch("{id:int}/status")]
    [RequireItemEngagementRole(ItemEntityType.HandoverReadinessItem, EngagementRoleEnum.PreSales)]
    [EndpointSummary("Update handover readiness item status (Pre-Sales only)")]
    [ProducesResponseType(typeof(HandoverReadinessItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverReadinessItemDto>> UpdateStatus(int id, [FromBody] UpdateHandoverReadinessStatusRequest request)
    {
        var item = await service.UpdateItemStatusAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Soft deletes a Handover Readiness item (Pre-Sales role required).
    /// </summary>
    /// <param name="id">The item ID.</param>
    [HttpDelete("{id:int}")]
    [RequireItemEngagementRole(ItemEntityType.HandoverReadinessItem, EngagementRoleEnum.PreSales)]
    [EndpointSummary("Delete handover readiness item (Pre-Sales only)")]
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
