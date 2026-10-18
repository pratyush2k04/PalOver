using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides engagement type lookup data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EngagementTypesController(IEngagementService engagementService) : ControllerBase
{
    /// <summary>
    /// Retrieves all available engagement types.
    /// </summary>
    /// <returns>A list of engagement types.</returns>
    [HttpGet]
    [EndpointSummary("Get all engagement types")]
    [ProducesResponseType(typeof(GetEngagementTypesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetEngagementTypesResponse>> GetAll()
    {
        var types = await engagementService.GetEngagementTypesAsync();
        return Ok(new GetEngagementTypesResponse(types));
    }
}
