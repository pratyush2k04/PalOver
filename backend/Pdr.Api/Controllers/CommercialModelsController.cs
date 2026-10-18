using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides commercial model lookup data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CommercialModelsController(IEngagementService engagementService) : ControllerBase
{
    /// <summary>
    /// Retrieves all available commercial models.
    /// </summary>
    /// <returns>A list of commercial models.</returns>
    [HttpGet]
    [EndpointSummary("Get all commercial models")]
    [ProducesResponseType(typeof(GetCommercialModelsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCommercialModelsResponse>> GetAll()
    {
        var models = await engagementService.GetCommercialModelsAsync();
        return Ok(new GetCommercialModelsResponse(models));
    }
}
