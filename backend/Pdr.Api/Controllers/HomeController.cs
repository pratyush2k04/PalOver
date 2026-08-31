using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides personalized home dashboard metrics and active engagements.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class HomeController(IHomeService homeService) : ControllerBase
{
    /// <summary>
    /// Retrieves home dashboard metrics and engagement summary for the logged-in user.
    /// </summary>
    /// <returns>Home dashboard metrics and active engagements.</returns>
    [HttpGet]
    [EndpointSummary("Get home dashboard data for authenticated user")]
    [ProducesResponseType(typeof(HomeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<HomeResponseDto>> GetHome()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var home = await homeService.GetHomeAsync(userId);
        return Ok(home);
    }

    /// <summary>
    /// Retrieves a page of engagements approaching kickoff for the logged-in user.
    /// </summary>
    /// <returns>A paged list of upcoming-kickoff engagements.</returns>
    [HttpGet("engagements")]
    [EndpointSummary("Get a page of dashboard engagements approaching kickoff")]
    [ProducesResponseType(typeof(PagedResult<DashboardEngagementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<DashboardEngagementDto>>> GetDashboardEngagements([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await homeService.GetDashboardEngagementsPageAsync(userId, page, pageSize);
        return Ok(result);
    }
}
