using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Kickoff;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages kickoff records, action items, and meeting outcomes.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/kickoff")]
[Produces("application/json")]
public class KickoffController(
    IKickoffService kickoffService,
    IValidator<SaveKickoffRecordRequest> recordValidator,
    IValidator<KickoffActionItemDto> actionItemValidator) : ControllerBase
{
    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Retrieves the kickoff record and action items for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>The kickoff record with action items and outcomes.</returns>
    [HttpGet]
    [Authorize(Roles = "Project Management,Tech Lead,Pre Sales")]
    [EndpointSummary("Get kickoff record")]
    [ProducesResponseType(typeof(KickoffRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<KickoffRecordDto>> GetKickoffRecord(int engagementId)
    {
        var record = await kickoffService.GetKickoffRecordAsync(engagementId, GetUserId());
        return Ok(record);
    }

    /// <summary>
    /// Saves outcomes and notes for the kickoff meeting record.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The request containing meeting outcomes.</param>
    /// <returns>The updated kickoff record.</returns>
    [HttpPost]
    [Authorize(Roles = "Project Management")]
    [EndpointSummary("Save kickoff meeting outcomes")]
    [ProducesResponseType(typeof(KickoffRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<KickoffRecordDto>> SaveKickoffRecord(int engagementId, [FromBody] SaveKickoffRecordRequest request)
    {
        var validationResult = await recordValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var savedRecord = await kickoffService.SaveKickoffRecordAsync(engagementId, request, GetUserId());
        return Ok(savedRecord);
    }

    /// <summary>
    /// Retrieves open (non-completed) action items for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>List of open action items.</returns>
    [HttpGet("actions/open")]
    [Authorize(Roles = "Project Management,Tech Lead,Pre Sales")]
    [EndpointSummary("Get open action items")]
    [ProducesResponseType(typeof(IEnumerable<KickoffActionItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<KickoffActionItemDto>>> GetOpenActionItems(int engagementId)
    {
        var openActions = await kickoffService.GetOpenActionItemsAsync(engagementId, GetUserId());
        return Ok(openActions);
    }

    /// <summary>
    /// Creates a new kickoff action item for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="dto">The action item details.</param>
    /// <returns>The created action item with assigned ID.</returns>
    [HttpPost("actions")]
    [Authorize(Roles = "Project Management")]
    [EndpointSummary("Create action item")]
    [ProducesResponseType(typeof(KickoffActionItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<KickoffActionItemDto>> CreateActionItem(int engagementId, [FromBody] KickoffActionItemDto dto)
    {
        var validationResult = await actionItemValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var created = await kickoffService.CreateActionItemAsync(engagementId, dto, GetUserId());
        return CreatedAtAction(nameof(GetKickoffRecord), new { engagementId }, created);
    }

    /// <summary>
    /// Updates the status of a kickoff action item.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="actionItemId">The action item ID.</param>
    /// <param name="request">The new status value.</param>
    /// <returns>The updated action item.</returns>
    [HttpPatch("actions/{actionItemId:int}/status")]
    [Authorize(Roles = "Project Management")]
    [EndpointSummary("Update action item status")]
    [ProducesResponseType(typeof(KickoffActionItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KickoffActionItemDto>> UpdateActionItemStatus(int engagementId, int actionItemId, [FromBody] UpdateActionItemStatusRequest request)
    {
        var updated = await kickoffService.UpdateActionItemStatusAsync(engagementId, actionItemId, request.Status, GetUserId());
        return Ok(updated);
    }

    /// <summary>
    /// Updates an existing kickoff action item.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="actionItemId">The action item ID.</param>
    /// <param name="dto">The updated action item details.</param>
    /// <returns>The updated action item.</returns>
    [HttpPut("actions/{actionItemId:int}")]
    [Authorize(Roles = "Project Management")]
    [EndpointSummary("Update action item")]
    [ProducesResponseType(typeof(KickoffActionItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KickoffActionItemDto>> UpdateActionItem(int engagementId, int actionItemId, [FromBody] KickoffActionItemDto dto)
    {
        var validationResult = await actionItemValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var updated = await kickoffService.UpdateActionItemAsync(engagementId, actionItemId, dto, GetUserId());
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a kickoff action item.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="actionItemId">The action item ID to delete.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("actions/{actionItemId:int}")]
    [Authorize(Roles = "Project Management")]
    [EndpointSummary("Delete action item")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActionItem(int engagementId, int actionItemId)
    {
        //await kickoffService.DeleteActionItemAsync(engagementId, actionItemId, GetUserId());
        return NoContent();
    }
}
