using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Workspace;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController(IWorkspaceService workspaceService) : ControllerBase
{
    //TODO: Authentication & Authorization constraints : Depends on 645
    [HttpGet("get-engagement-types")]
    public async Task<ActionResult<GetEngagementTypesResponse>> GetEngagementTypes()
    {
        var types = await workspaceService.GetEngagementTypesAsync();
        return Ok(new GetEngagementTypesResponse(types));
    }

    //TODO: Authentication & Authorization constraints : Depends on 645
    [HttpGet("get-engagement-info/{id:long}")]
    public async Task<ActionResult<EngagementInfoDto>> GetEngagementInfo(long id)
    {
        var info = await workspaceService.GetEngagementInfoAsync(id);
        return info is null ? NotFound() : Ok(info);
    }

    //TODO: Authentication & Authorization constraints : Depends on 645
    [HttpPut("edit-engagement-info/{id:long}")]
    public async Task<IActionResult> EditEngagementInfo(long id, EditEngagementRequest request)
    {
        var success = await workspaceService.EditEngagementAsync(id, request);
        return success ? NoContent() : NotFound();
    }
}