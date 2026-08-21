using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.DependencySpikes;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages proof-of-concept dependency spikes for third-party tools, libraries, and SDKs.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/dependency-spikes")]
[Produces("application/json")]
public class DependencySpikesController(IDependencySpikeService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all dependency spikes for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>List of dependency spikes.</returns>
    [HttpGet]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership, RoleEnum.PreSales)]
    [EndpointSummary("Get all dependency spikes for an engagement")]
    [ProducesResponseType(typeof(List<DependencySpikeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpikes(int engagementId)
    {
        var result = await service.GetSpikesByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new dependency spike for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The creation payload.</param>
    /// <returns>The created dependency spike.</returns>
    [HttpPost]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    [EndpointSummary("Create dependency spike")]
    [ProducesResponseType(typeof(DependencySpikeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSpike(int engagementId, [FromBody] CreateDependencySpikeRequest request)
    {
        var result = await service.CreateSpikeAsync(engagementId, request);
        return CreatedAtAction(nameof(GetSpikes), new { engagementId }, result);
    }

    /// <summary>
    /// Updates an existing dependency spike.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The spike ID.</param>
    /// <param name="request">The update payload.</param>
    /// <returns>The updated dependency spike.</returns>
    [HttpPut("{id:int}")]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    [EndpointSummary("Update dependency spike")]
    [ProducesResponseType(typeof(DependencySpikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSpike(int engagementId, int id, [FromBody] UpdateDependencySpikeRequest request)
    {
        var result = await service.UpdateSpikeAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a dependency spike.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The spike ID.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("{id:int}")]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    [EndpointSummary("Delete dependency spike")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSpike(int engagementId, int id)
    {
        await service.DeleteSpikeAsync(id);
        return NoContent();
    }
}
