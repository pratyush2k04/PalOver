using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Exceptions;
using Pdr.Api.Exceptions.AccessItemExceptions;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

using Pdr.Api.Constants;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route(AccessTrackerConstants.Routes.Base)]
public class AccessTrackerController(IAccessTrackerService accessTrackerService) : ControllerBase
{
    // GET api/engagements/{engagementId}/access-items
    // Tech Lead, Project Management, Leadership — view access
    [HttpGet]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    public async Task<IActionResult> GetAccessItems(int engagementId)
    {
        var items = await accessTrackerService.GetAccessItemsAsync(engagementId);
        return Ok(items);
    }

    // POST api/engagements/{engagementId}/access-items
    // Tech Lead — edit access
    [HttpPost]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> AddCustomAccessItem(int engagementId, [FromBody] AddCustomAccessItemRequest request)
    {
        var item = await accessTrackerService.AddCustomAccessItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetAccessItems), new { engagementId }, item);
    }

    // POST api/engagements/{engagementId}/access-items/evaluate-risks
    // Test endpoint for User Story 3 — triggers the 2-day billing risk rule manually
    [HttpPost("evaluate-risks")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> EvaluateClientSideRisks(int engagementId)
    {
        await accessTrackerService.EvaluateClientSideRisksAsync(engagementId);
        return Ok(new { message = "Client-side risk evaluation completed." });
    }
}

// Separate controller for item-level operations (by item id, not engagement id)
[ApiController]
[Authorize]
[Route(AccessTrackerConstants.Routes.ItemsBase)]
public class AccessItemsController(IAccessTrackerService accessTrackerService) : ControllerBase
{
    // PATCH api/access-items/{id}/status
    // Tech Lead — edit access
    [HttpPatch("{id:int}/status")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAccessItemStatusRequest request)
    {
        var item = await accessTrackerService.UpdateAccessItemStatusAsync(id, request);
        return Ok(item);
    }

    // PATCH api/access-items/{id}/owner
    // Tech Lead — edit access
    [HttpPatch("{id:int}/owner")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] UpdateAccessItemOwnerRequest request)
    {
        var item = await accessTrackerService.UpdateAccessItemOwnerAsync(id, request);
        return Ok(item);
    }
}
