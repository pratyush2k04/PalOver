using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommercialModelsController(IEngagementService engagementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GetCommercialModelsResponse>> GetAll()
    {
        var models = await engagementService.GetCommercialModelsAsync();
        return Ok(new GetCommercialModelsResponse(models));
    }
}
