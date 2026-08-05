using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.EnvironmentTooling;
using Pdr.Api.Exceptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/environment-tooling")]
public class EnvironmentToolingController(IEnvironmentToolingService service) : ControllerBase
{
    // GET api/engagements/{engagementId}/environment-tooling
    // Tech Lead, Project Management, Leadership — view access
    [HttpGet]
    [Authorize(Roles = "Tech Lead,Project Management,Leadership")]
    public async Task<IActionResult> GetItems(int engagementId)
    {
        try
        {
            var response = await service.GetItemsByEngagementIdAsync(engagementId);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // POST api/engagements/{engagementId}/environment-tooling
    // Tech Lead — edit access
    [HttpPost]
    [Authorize(Roles = "Tech Lead")]
    public async Task<IActionResult> AddCustomItem(int engagementId, [FromBody] AddCustomEnvironmentToolingItemRequest request)
    {
        try
        {
            var item = await service.AddCustomItemAsync(engagementId, request);
            return CreatedAtAction(nameof(GetItems), new { engagementId }, item);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}

// Item-level routes by item ID
[ApiController]
[Authorize]
[Route("api/environment-tooling")]
public class EnvironmentToolingItemsController(IEnvironmentToolingService service) : ControllerBase
{
    // PATCH api/environment-tooling/{id}/status
    // Tech Lead — edit access
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Tech Lead")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateEnvironmentToolingStatusRequest request)
    {
        try
        {
            var item = await service.UpdateStatusAsync(id, request);
            return Ok(item);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // PATCH api/environment-tooling/{id}/owner
    // Tech Lead — edit access
    [HttpPatch("{id:int}/owner")]
    [Authorize(Roles = "Tech Lead")]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] UpdateEnvironmentToolingOwnerRequest request)
    {
        try
        {
            var item = await service.UpdateOwnerAsync(id, request);
            return Ok(item);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
