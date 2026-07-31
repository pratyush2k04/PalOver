using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngagementTypesController(IEngagementService engagementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GetEngagementTypesResponse>> GetAll()
    {
        var types = await engagementService.GetEngagementTypesAsync();
        return Ok(new GetEngagementTypesResponse(types));
    }
}
