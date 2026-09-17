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
[Route("api/engagements/{engagementId:int}/workspace/dependencies")]
[Authorize]
[RequireEngagementAccess]
[Produces("application/json")]
public class DependenciesController(IDependencyService dependencyService) : ControllerBase
{
    /// <summary>
    /// Retrieves dependencies for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    [HttpGet]
    [EndpointSummary("Get all dependencies for an engagement")]
    [ProducesResponseType(typeof(IEnumerable<DependencyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<DependencyDto>>> GetDependencies(int engagementId)
    {
        var result = await dependencyService.GetDependenciesAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new dependency.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The creation payload.</param>
    [HttpPost]
    [EndpointSummary("Create a new dependency")]
    [ProducesResponseType(typeof(DependencyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DependencyDto>> CreateDependency(int engagementId, [FromBody] CreateDependencyDto request)
    {
        var result = await dependencyService.CreateDependencyAsync(engagementId, request);
        return CreatedAtAction(nameof(GetDependencies), new { engagementId }, result);
    }

    /// <summary>
    /// Updates an existing dependency.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The dependency ID.</param>
    /// <param name="request">The update payload.</param>
    [HttpPut("{id}")]
    [EndpointSummary("Update a dependency")]
    [ProducesResponseType(typeof(DependencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DependencyDto>> UpdateDependency(int engagementId, int id, [FromBody] UpdateDependencyDto request)
    {
        var result = await dependencyService.UpdateDependencyAsync(engagementId, id, request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a dependency.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The dependency ID.</param>
    [HttpDelete("{id}")]
    [EndpointSummary("Delete a dependency")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDependency(int engagementId, int id)
    {
        await dependencyService.DeleteDependencyAsync(engagementId, id);
        return NoContent();
    }

}
