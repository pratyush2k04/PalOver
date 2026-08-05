using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.EngagementChecklist;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/engagements/{engagementId:int}/workspace/checklist")]
[Authorize]
public class EngagementChecklistController(IEngagementChecklistService engagementChecklistService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<EngagementChecklistResponseDto>> GetChecklist(int engagementId)
    {
        var result = await engagementChecklistService.GetByEngagementIdAsync(engagementId);
        return Ok(result);
    }
}
