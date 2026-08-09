using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.EnvironmentTooling;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

using Pdr.Api.Constants;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route(EnvironmentToolingConstants.Routes.Base)]
public class EnvironmentToolingController(IEnvironmentToolingService service) : ControllerBase
{
    // GET api/engagements/{engagementId}/environment-tooling
    // Tech Lead, Project Management, Leadership — view access
    [HttpGet]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    public async Task<IActionResult> GetItems(int engagementId)
    {
        var response = await service.GetItemsByEngagementIdAsync(engagementId);
        return Ok(response);
    }

    // POST api/engagements/{engagementId}/environment-tooling
    // Tech Lead — edit access
    [HttpPost]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> AddCustomItem(int engagementId, [FromBody] AddCustomEnvironmentToolingItemRequest request)
    {
        var item = await service.AddCustomItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetItems), new { engagementId }, item);
    }
}

// Item-level routes by item ID
[ApiController]
[Authorize]
[Route(EnvironmentToolingConstants.Routes.ItemsBase)]
public class EnvironmentToolingItemsController(IEnvironmentToolingService service) : ControllerBase
{
    // PATCH api/environment-tooling/{id}/status
    // Tech Lead — edit access
    [HttpPatch("{id:int}/status")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateEnvironmentToolingStatusRequest request)
    {
        var item = await service.UpdateStatusAsync(id, request);
        return Ok(item);
    }

    // PATCH api/environment-tooling/{id}/owner
    // Tech Lead — edit access
    [HttpPatch("{id:int}/owner")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] UpdateEnvironmentToolingOwnerRequest request)
    {
        var item = await service.UpdateOwnerAsync(id, request);
        return Ok(item);
    }
}
