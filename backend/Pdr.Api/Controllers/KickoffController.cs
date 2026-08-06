using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Kickoff;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/engagements/{engagementId:int}/kickoff")]
public class KickoffController(
    IKickoffService kickoffService,
    IValidator<SaveKickoffRecordRequest> recordValidator,
    IValidator<KickoffActionItemDto> actionItemValidator) : ControllerBase
{
    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    [Authorize(Roles = "Project Management,Tech Lead,Pre Sales")]
    public async Task<ActionResult<KickoffRecordDto>> GetKickoffRecord(int engagementId)
    {
        var record = await kickoffService.GetKickoffRecordAsync(engagementId, GetUserId());
        return Ok(record);
    }

    [HttpPost]
    [Authorize(Roles = "Project Management")]
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

    [HttpGet("actions/open")]
    [Authorize(Roles = "Project Management,Tech Lead,Pre Sales")]
    public async Task<ActionResult<IEnumerable<KickoffActionItemDto>>> GetOpenActionItems(int engagementId)
    {
        var openActions = await kickoffService.GetOpenActionItemsAsync(engagementId, GetUserId());
        return Ok(openActions);
    }

    [HttpPost("actions")]
    [Authorize(Roles = "Project Management")]
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

    [HttpPatch("actions/{actionItemId:int}/status")]
    [Authorize(Roles = "Project Management")]
    public async Task<ActionResult<KickoffActionItemDto>> UpdateActionItemStatus(int engagementId, int actionItemId, [FromBody] UpdateActionItemStatusRequest request)
    {
        var updated = await kickoffService.UpdateActionItemStatusAsync(engagementId, actionItemId, request.Status, GetUserId());
        return Ok(updated);
    }

    [HttpPut("actions/{actionItemId:int}")]
    [Authorize(Roles = "Project Management")]
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

    [HttpDelete("actions/{actionItemId:int}")]
    [Authorize(Roles = "Project Management")]
    public async Task<IActionResult> DeleteActionItem(int engagementId, int actionItemId)
    {
        await kickoffService.DeleteActionItemAsync(engagementId, actionItemId, GetUserId());
        return NoContent();
    }
}
