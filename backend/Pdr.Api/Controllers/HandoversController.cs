using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/engagements/{engagementId:long}/handover")]
public class HandoversController : ControllerBase
{
    private readonly IHandoverService _handoverService;
    private readonly IHandoverAccessService _accessService;
    private readonly IValidator<SaveHandoverDraftRequest> _saveDraftValidator;

    public HandoversController(
        IHandoverService handoverService,
        IHandoverAccessService accessService,
        IValidator<SaveHandoverDraftRequest> saveDraftValidator)
    {
        _handoverService = handoverService;
        _accessService = accessService;
        _saveDraftValidator = saveDraftValidator;
    }

    private int GetUserId() 
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 1; // Fallback to User 1 (Pre-Sales) for testing without Auth
    }

    private string GetUserIdString()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? claim.Value : "1"; // Fallback for testing
    }

    [HttpGet]
    public async Task<ActionResult<HandoverInfoDto>> GetHandover(long engagementId)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var dto = await _handoverService.GetHandoverAsync(engagementId);
        return Ok(dto);
    }

    [HttpPut("draft")]
    public async Task<ActionResult<HandoverInfoDto>> SaveDraft(long engagementId, [FromBody] SaveHandoverDraftRequest request)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var validationResult = await _saveDraftValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid handover draft request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _handoverService.SaveHandoverDraftAsync(engagementId, request, GetUserIdString());
        return Ok(dto);
    }

    [HttpPost("submit")]
    public async Task<ActionResult<HandoverInfoDto>> Submit(long engagementId)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var dto = await _handoverService.SubmitHandoverAsync(engagementId, GetUserIdString());
        return Ok(dto);
    }
}
