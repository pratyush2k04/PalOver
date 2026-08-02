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
    [HttpGet("my-engagements")]
    [Authorize(Roles = "Project Management")]
    public async Task<ActionResult<PmEngagementListResponse>> GetMyEngagements([FromQuery] string sortBy = "kickoff")
    {
        var result = await engagementService.GetMyEngagementsAsync(sortBy);
        return Ok(result);
    }

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
        try
        {
            var success = await engagementService.EditEngagementAsync(id, request);
            return success ? NoContent() : NotFound();
        }
        catch (InvalidEngagementTypeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("mark-sow-signed")]
    [Authorize(Roles = "Pre Sales")]
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
        catch (InvalidEngagementTypeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:long}/owners")]
    [Authorize(Roles = "Leadership")]
    public async Task<ActionResult<EngagementOwnersDto>> GetEngagementOwners(long id)
    {
        var owners = await engagementService.GetEngagementOwnersAsync(id);
        return Ok(owners);
    }
    //assign
    [HttpPut("{id:long}/project-manager")]
    [Authorize(Roles = "Leadership")]
    public async Task<ActionResult<EngagementOwnersDto>> AssignProjectManager(long id, AssignOwnerRequest request)
    {
        var owners = await engagementService.AssignProjectManagerAsync(id, request.UserId);
        return Ok(owners);
    }

    [HttpPut("{id:long}/tech-lead")]
    [Authorize(Roles = "Leadership")]
    public async Task<ActionResult<EngagementOwnersDto>> AssignTechLead(long id, AssignOwnerRequest request)
    {
        var owners = await engagementService.AssignTechLeadAsync(id, request.UserId);
        return Ok(owners);
    }
}
