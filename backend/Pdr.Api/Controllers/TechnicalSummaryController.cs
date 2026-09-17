using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.TechnicalSummary;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages Consolidated Day-1 Technical Readiness Summary for an engagement.
/// </summary>
[ApiController]
[Authorize]
[RequireEngagementAccess]
[Route("api/engagements/{engagementId:int}/technical-readiness/summary")]
[Produces("application/json")]
public class TechnicalSummaryController(ITechnicalSummaryService service) : ControllerBase
{
    /// <summary>
    /// Retrieves the consolidated Day-1 technical readiness summary for an engagement.
    /// Excludes all PM-only commercial/financial data and provides a consolidated view of Access, Tooling, Architecture, and Brownfield readiness.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>The consolidated Day-1 readiness summary.</returns>
    [HttpGet]
    [EndpointSummary("Get consolidated Day-1 technical readiness summary")]
    [ProducesResponseType(typeof(DayOneReadinessSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary(int engagementId)
    {
        var result = await service.GetDayOneSummaryAsync(engagementId);
        return Ok(result);
    }
}
