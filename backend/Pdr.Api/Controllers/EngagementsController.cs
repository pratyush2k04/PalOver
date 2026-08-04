using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Exceptions;

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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EngagementInfoDto>> GetEngagementInfo(int id)
    {
        Console.WriteLine("GetEngagementInfo called with id: " + id);
        var info = await engagementService.GetEngagementInfoAsync(id);
        return info is null ? NotFound() : Ok(info);
    }

    //TODO: Authentication & Authorization constraints : Depends on 645
    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditEngagementInfo(int id, EditEngagementRequest request)
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
       
            var response = await engagementService.MarkSowSignedAsync(request);
            return Ok(response);
   
    }

    //assign
    [HttpGet("{id:int}/owners")]
    [Authorize(Roles = "Leadership")]
    public async Task<ActionResult<EngagementOwnersDto>> GetEngagementOwners(int id)
    {
        var owners = await engagementService.GetEngagementOwnersAsync(id);
        return Ok(owners);
    }
    [HttpPut("{id:int}/project-manager")]
    [Authorize(Roles = "Leadership")]
    public async Task<ActionResult<EngagementOwnersDto>> AssignProjectManager(int id, AssignOwnerRequest request)
    {
        var owners = await engagementService.AssignProjectManagerAsync(id, request.UserId);
        return Ok(owners);
    }

    [HttpPut("{id:int}/tech-lead")]
    [Authorize(Roles = "Leadership")]
    public async Task<ActionResult<EngagementOwnersDto>> AssignTechLead(int id, AssignOwnerRequest request)
    {
        var owners = await engagementService.AssignTechLeadAsync(id, request.UserId);
        return Ok(owners);
    }
}
