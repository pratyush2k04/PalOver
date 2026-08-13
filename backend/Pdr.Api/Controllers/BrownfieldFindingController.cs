using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.BrownField;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages Brownfield Review findings for an engagement.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/brownfield-findings")]
[Produces("application/json")]
public class BrownfieldFindingController(IBrownfieldFindingService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all brownfield review findings for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>List of findings and readiness summary.</returns>
    [HttpGet]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    [EndpointSummary("Get brownfield review findings")]
    [ProducesResponseType(typeof(BrownfieldReadinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFindings(int engagementId)
    {
        var response = await service.GetFindingsByEngagementIdAsync(engagementId);
        return Ok(response);
    }

    /// <summary>
    /// Creates a new brownfield finding for an engagement (Tech Lead only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The finding creation request payload.</param>
    /// <returns>The created finding.</returns>
    [HttpPost]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Create brownfield finding (Tech Lead only)")]
    [ProducesResponseType(typeof(BrownfieldFindingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFinding(int engagementId, [FromBody] CreateBrownfieldFindingRequest request)
    {
        var item = await service.CreateFindingAsync(engagementId, request);
        return CreatedAtAction(nameof(GetFindings), new { engagementId }, item);
    }
}

/// <summary>
/// Manages finding-level operations for individual brownfield findings.
/// </summary>
[ApiController]
[Authorize]
[Route("api/brownfield-findings")]
[Produces("application/json")]
public class BrownfieldFindingItemController(IBrownfieldFindingService service) : ControllerBase
{
    /// <summary>
    /// Updates the client decision for a brownfield finding (Tech Lead only).
    /// </summary>
    /// <param name="id">The finding ID.</param>
    /// <param name="request">The decision update payload (AcceptNow = 1, DeferLater = 2).</param>
    /// <returns>The updated finding.</returns>
    [HttpPatch("{id:int}/decision")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Update client decision for finding (Tech Lead only)")]
    [ProducesResponseType(typeof(BrownfieldFindingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDecision(int id, [FromBody] UpdateClientDecisionRequest request)
    {
        var item = await service.UpdateClientDecisionAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Soft-deletes a brownfield finding (Tech Lead only).
    /// </summary>
    /// <param name="id">The finding ID.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("{id:int}")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Delete brownfield finding (Tech Lead only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFinding(int id)
    {
        await service.DeleteFindingAsync(id);
        return NoContent();
    }
}
