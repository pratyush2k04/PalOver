using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Services.Interfaces;

using Pdr.Api.Constants;

namespace Pdr.Api.Controllers;

[ApiController]
[Route(HomeConstants.Routes.Base)]
[Authorize]
public class HomeController(IHomeService homeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HomeResponseDto>> GetHome()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var home = await homeService.GetHomeAsync(userId);
        return Ok(home);
    }
}
