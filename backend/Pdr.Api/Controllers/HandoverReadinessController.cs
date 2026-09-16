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

[ApiController]
[Authorize]
public class HandoverReadinessController(IHandoverReadinessService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all Handover Readiness items for an engagement.
    /// Seeds default items if they do not exist yet.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>List of Handover Readiness items.</returns>
    [HttpGet("api/engagements/{engagementId:int}/handover-readiness")]
    [ProducesResponseType(typeof(List<HandoverReadinessItemDto>), StatusCodes.Status200OK)]
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
    [HttpPost("api/engagements/{engagementId:int}/handover-readiness")]
    [AuthorizeRoles(UserRoleEnum.PreSales)]
    [ProducesResponseType(typeof(HandoverReadinessItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverReadinessItemDto>> AddCustomItem(int engagementId, [FromBody] AddCustomHandoverReadinessItemRequest request)
    {
        var item = await service.AddCustomItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetItems), new { engagementId }, item);
    }

    /// <summary>
    /// Updates the status of a Handover Readiness item (Pre-Sales role required).
    /// Syncs risks if marked as Blocked or unblocked.
    /// </summary>
    /// <param name="id">The item ID.</param>
    /// <param name="request">The status update request.</param>
    /// <returns>The updated Handover Readiness item.</returns>
    [HttpPatch("api/handover-readiness/{id:int}/status")]
    [AuthorizeRoles(UserRoleEnum.PreSales)]
    [ProducesResponseType(typeof(HandoverReadinessItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [HttpDelete("api/handover-readiness/{id:int}")]
    [AuthorizeRoles(UserRoleEnum.PreSales)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await service.DeleteItemAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Attaches or updates an evidence document for a Handover Readiness item (Pre-Sales role required).
    /// </summary>
    [HttpPut("api/engagements/{engagementId:int}/handover-readiness/{itemId:int}/evidence")]
    [AuthorizeRoles(UserRoleEnum.PreSales)]
    [EndpointSummary("Attach or update handover readiness item evidence")]
    [ProducesResponseType(typeof(HandoverReadinessItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverReadinessItemDto>> SaveEvidence(int engagementId, int itemId, [FromBody] SaveHandoverReadinessEvidenceRequestDto request)
    {
        var item = await service.SaveItemEvidenceAsync(engagementId, itemId, request);
        return Ok(item);
    }
}
