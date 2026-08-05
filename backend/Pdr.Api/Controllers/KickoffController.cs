using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Kickoff;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/engagements/{engagementId:int}/kickoff")]
public class KickoffController(
    IKickoffService kickoffService,
    IValidator<SaveKickoffRecordRequest> validator) : ControllerBase
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
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var savedRecord = await kickoffService.SaveKickoffRecordAsync(engagementId, request, GetUserId());
        return Ok(savedRecord);
    }
}
