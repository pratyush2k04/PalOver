using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Exceptions;
using Pdr.Api.Exceptions.AccessItemExceptions;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/access-items")]
public class AccessTrackerController(IAccessTrackerService accessTrackerService) : ControllerBase
{
    // GET api/engagements/{engagementId}/access-items
    // Tech Lead, Project Management, Leadership — view access
    [HttpGet]
    [Authorize(Roles = $"{RoleNames.TechLead},{RoleNames.ProjectManagement},{RoleNames.Leadership}")]
    public async Task<IActionResult> GetAccessItems(int engagementId)
    {
        try
        {
            var items = await accessTrackerService.GetAccessItemsAsync(engagementId);
            return Ok(items);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // POST api/engagements/{engagementId}/access-items
    // Tech Lead — edit access
    [HttpPost]
    [Authorize(Roles = RoleNames.TechLead)]
    public async Task<IActionResult> AddCustomAccessItem(int engagementId, [FromBody] AddCustomAccessItemRequest request)
    {
        try
        {
            var item = await accessTrackerService.AddCustomAccessItemAsync(engagementId, request);
            return CreatedAtAction(nameof(GetAccessItems), new { engagementId }, item);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // POST api/engagements/{engagementId}/access-items/evaluate-risks
    // Test endpoint for User Story 3 — triggers the 2-day billing risk rule manually
    [HttpPost("evaluate-risks")]
    [Authorize(Roles = RoleNames.TechLead)]
    public async Task<IActionResult> EvaluateClientSideRisks(int engagementId)
    {
        try
        {
            await accessTrackerService.EvaluateClientSideRisksAsync(engagementId);
            return Ok(new { message = "Client-side risk evaluation completed." });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}

// Separate controller for item-level operations (by item id, not engagement id)
[ApiController]
[Authorize]
[Route("api/access-items")]
public class AccessItemsController(IAccessTrackerService accessTrackerService) : ControllerBase
{
    // PATCH api/access-items/{id}/status
    // Tech Lead — edit access
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = RoleNames.TechLead)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAccessItemStatusRequest request)
    {
        try
        {
            var item = await accessTrackerService.UpdateAccessItemStatusAsync(id, request);
            return Ok(item);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // PATCH api/access-items/{id}/owner
    // Tech Lead — edit access
    [HttpPatch("{id:int}/owner")]
    [Authorize(Roles = RoleNames.TechLead)]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] UpdateAccessItemOwnerRequest request)
    {
        try
        {
            var item = await accessTrackerService.UpdateAccessItemOwnerAsync(id, request);
            return Ok(item);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
