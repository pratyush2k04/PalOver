using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
}
