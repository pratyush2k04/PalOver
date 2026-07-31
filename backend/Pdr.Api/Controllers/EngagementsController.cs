using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngagementsController(IEngagementService engagementService) : ControllerBase
{
    //TODO: Authentication & Authorization constraints : Depends on 645
    [HttpGet("{id:long}")]
    public async Task<ActionResult<EngagementInfoDto>> GetEngagementInfo(long id)
    {
        var info = await engagementService.GetEngagementInfoAsync(id);
        return info is null ? NotFound() : Ok(info);
    }

    //TODO: Authentication & Authorization constraints : Depends on 645
    [HttpPut("{id:long}")]
    public async Task<IActionResult> EditEngagementInfo(long id, EditEngagementRequest request)
    {
        var success = await engagementService.EditEngagementAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("mark-sow-signed")]
    // TODO: restore once the auth scheme (teammate's work) is merged
    // [Authorize(Roles = "Presales")]
    public async Task<ActionResult<MarkSowSignedResponse>> MarkSowSigned(MarkSowSignedRequest request)
    {
        try
        {
            var response = await engagementService.MarkSowSignedAsync(request);
            return Ok(response);
        }
        catch (BillingBeforeKickoffException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
