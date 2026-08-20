using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Dependencies;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Attributes;
using Pdr.Api.Services.Interfaces;
using System.Security.Claims;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages the dependencies register for an engagement.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId}/workspace/dependencies")]
[Authorize]
[Produces("application/json")]
public class DependenciesController(IDependencyService dependencyService) : ControllerBase
{
    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Retrieves dependencies for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    [HttpGet]
    [AuthorizeRoles(RoleEnum.ProjectManagement, RoleEnum.TechLead, RoleEnum.PreSales)]
    [EndpointSummary("Get all dependencies for an engagement")]
    [ProducesResponseType(typeof(IEnumerable<DependencyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<DependencyDto>>> GetDependencies(int engagementId)
    {
        var result = await dependencyService.GetDependenciesAsync(engagementId, CurrentUserId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new dependency.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The creation payload.</param>
    [HttpPost]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    [EndpointSummary("Create a new dependency")]
    [ProducesResponseType(typeof(DependencyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DependencyDto>> CreateDependency(int engagementId, [FromBody] CreateDependencyDto request)
    {
        var result = await dependencyService.CreateDependencyAsync(engagementId, request, CurrentUserId);
        return CreatedAtAction(nameof(GetDependencies), new { engagementId }, result);
    }

    /// <summary>
    /// Updates an existing dependency.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The dependency ID.</param>
    /// <param name="request">The update payload.</param>
    [HttpPut("{id}")]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    [EndpointSummary("Update a dependency")]
    [ProducesResponseType(typeof(DependencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DependencyDto>> UpdateDependency(int engagementId, int id, [FromBody] UpdateDependencyDto request)
    {
        var result = await dependencyService.UpdateDependencyAsync(engagementId, id, request, CurrentUserId);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a dependency.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The dependency ID.</param>
    [HttpDelete("{id}")]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    [EndpointSummary("Delete a dependency")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDependency(int engagementId, int id)
    {
        await dependencyService.DeleteDependencyAsync(engagementId, id, CurrentUserId);
        return NoContent();
    }

    /// <summary>
    /// Escalates an At Risk external dependency to the Risk register.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The dependency ID.</param>
    [HttpPost("{id}/escalate")]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    [EndpointSummary("Escalate an At Risk external dependency to a Risk")]
    [ProducesResponseType(typeof(DependencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DependencyDto>> EscalateDependency(int engagementId, int id)
    {
        var result = await dependencyService.EscalateToRiskAsync(engagementId, id, CurrentUserId);
        return Ok(result);
    }
}
