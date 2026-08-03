using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Services.Interfaces;
using System.Security.Claims;

namespace Pdr.Api.Controllers;

[ApiController]
// [Authorize] - Commented out for now as per EngagementsController
[Route("api/engagements/{engagementId:long}/handover")]
public class HandoversController : ControllerBase
{
    private readonly IHandoverService _handoverService;
    private readonly IHandoverAccessService _accessService;

    public HandoversController(IHandoverService handoverService, IHandoverAccessService accessService)
    {
        _handoverService = handoverService;
        _accessService = accessService;
    }

    [HttpGet]
    public async Task<ActionResult<HandoverInfoDto>> GetHandover(long engagementId)
    {
        // Extract userId from claims, defaulting to 1 for dev if auth is merged
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int userId = 1; 
        if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int parsed))
        {
            userId = parsed;
        }

        await _accessService.CanAccessHandoverAsync(engagementId, userId);

        var dto = await _handoverService.GetHandoverAsync(engagementId);
        return Ok(dto);
    }

    [HttpPut("draft")]
    public async Task<ActionResult<HandoverInfoDto>> SaveDraft(long engagementId, [FromBody] SaveHandoverDraftRequest request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1";
        if (int.TryParse(userIdStr, out int userId))
        {
            await _accessService.CanAccessHandoverAsync(engagementId, userId);
        }

        var dto = await _handoverService.SaveHandoverDraftAsync(engagementId, request, userIdStr);
        return Ok(dto);
    }

    [HttpPost("submit")]
    public async Task<ActionResult<HandoverInfoDto>> Submit(long engagementId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1";
        if (int.TryParse(userIdStr, out int userId))
        {
            await _accessService.CanAccessHandoverAsync(engagementId, userId);
        }

        try
        {
            var dto = await _handoverService.SubmitHandoverAsync(engagementId, userIdStr);
            return Ok(dto);
        }
        catch (System.Exception ex) when (ex.Message.Contains("required") || ex.Message.Contains("must either"))
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
