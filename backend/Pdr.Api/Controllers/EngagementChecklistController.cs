using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.EngagementChecklist;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages engagement readiness checklist items, applicability, and exemption reviews.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/workspace/checklist")]
[Authorize]
[Produces("application/json")]
public class EngagementChecklistController(IEngagementChecklistService engagementChecklistService) : ControllerBase
{
    /// <summary>
    /// Retrieves the engagement readiness checklist.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>The engagement checklist items and summary.</returns>
    [HttpGet]
    [EndpointSummary("Get engagement checklist")]
    [ProducesResponseType(typeof(EngagementChecklistResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> GetChecklist(int engagementId)
    {
        var result = await engagementChecklistService.GetByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Updates the completion status of a checklist item (Project Management &amp; Tech Lead).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="itemId">The checklist item ID.</param>
    /// <param name="dto">The status update payload.</param>
    /// <returns>The updated engagement checklist.</returns>
    [HttpPatch("items/{itemId:int}/status")]
    [EndpointSummary("Update checklist item completion status")]
    [ProducesResponseType(typeof(EngagementChecklistResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> UpdateItemStatus(
        int engagementId,
        int itemId,
        [FromBody] UpdateItemStatusRequestDto dto)
    {
        var result = await engagementChecklistService.UpdateItemStatusAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Updates the applicability or requests exemption for a checklist item (Project Management only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="itemId">The checklist item ID.</param>
    /// <param name="dto">The applicability update payload.</param>
    /// <returns>The updated engagement checklist.</returns>
    [HttpPost("items/{itemId:int}/applicability")]
    [EndpointSummary("Update checklist item applicability or request exemption (PM only)")]
    [ProducesResponseType(typeof(EngagementChecklistResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> UpdateItemApplicability(
        int engagementId,
        int itemId,
        [FromBody] UpdateItemApplicabilityRequestDto dto)
    {
        var result = await engagementChecklistService.UpdateItemApplicabilityAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Reviews (approves or rejects) a pending exemption request (Leadership only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="itemId">The checklist item ID.</param>
    /// <param name="dto">The exemption review payload.</param>
    /// <returns>The updated engagement checklist.</returns>
    [HttpPost("items/{itemId:int}/exemption/review")]
    [EndpointSummary("Review exemption request (Leadership only)")]
    [ProducesResponseType(typeof(EngagementChecklistResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> ReviewExemption(
        int engagementId,
        int itemId,
        [FromBody] ReviewExemptionRequestDto dto)
    {
        var result = await engagementChecklistService.ReviewExemptionAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    [HttpPatch("items/{itemId:int}/owner")]
    public async Task<ActionResult<EngagementChecklistResponseDto>> UpdateItemOwner(
        int engagementId,
        int itemId,
        [FromBody] UpdateItemOwnerRequestDto dto)
    {
        var result = await engagementChecklistService.UpdateItemOwnerAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    [HttpPost("items/{itemId:int}/block")]
    public async Task<ActionResult<EngagementChecklistResponseDto>> BlockItem(
        int engagementId,
        int itemId,
        [FromBody] BlockChecklistItemRequestDto dto)
    {
        var result = await engagementChecklistService.BlockItemAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    [HttpPost("items/{itemId:int}/unblock")]
    public async Task<ActionResult<EngagementChecklistResponseDto>> UnblockItem(int engagementId, int itemId)
    {
        var result = await engagementChecklistService.UnblockItemAsync(engagementId, itemId);
        return Ok(result);
    }

    /// <summary>
    /// Attaches or updates evidence on a checklist item and optionally marks it Complete.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="itemId">The checklist item ID.</param>
    /// <param name="dto">The evidence payload.</param>
    /// <returns>The updated engagement checklist.</returns>
    [HttpPut("items/{itemId:int}/evidence")]
    [EndpointSummary("Attach or update checklist item evidence")]
    [ProducesResponseType(typeof(EngagementChecklistResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> SaveItemEvidence(
        int engagementId,
        int itemId,
        [FromBody] SaveChecklistItemEvidenceRequestDto dto)
    {
        var result = await engagementChecklistService.SaveItemEvidenceAsync(engagementId, itemId, dto);
        return Ok(result);
    }
}
