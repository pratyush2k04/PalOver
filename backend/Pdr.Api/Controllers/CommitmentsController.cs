using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages commitments and realistic commitments for engagement handovers.
/// </summary>
[ApiController]
[Authorize]
[RequireEngagementAccess]
[Route("api/engagements/{engagementId:int}/handover/commitments")]
[Produces("application/json")]
public class CommitmentsController(
    ICommitmentService commitmentService,
    IRealisticCommitmentService realisticCommitmentService) : ControllerBase
{
    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    /// <summary>
    /// Adds a new commitment to the handover (Pre Sales only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="request">The commitment data to add.</param>
    /// <returns>The created commitment.</returns>
    [HttpPost]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Add commitment to handover (Pre Sales only)")]
    [ProducesResponseType(typeof(CommitmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CommitmentDto>> AddCommitment(int engagementId, [FromBody] CreateCommitmentRequest request)
    {
        var dto = await commitmentService.AddCommitmentAsync(engagementId, request, GetUserId());
        return CreatedAtAction(nameof(AddCommitment), new { engagementId, commitmentId = dto.CommitmentId }, dto);
    }

    /// <summary>
    /// Updates an existing commitment (Pre Sales only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment to update.</param>
    /// <param name="request">The updated commitment data.</param>
    /// <returns>The updated commitment.</returns>
    [HttpPut("{commitmentId:int}")]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Update commitment (Pre Sales only)")]
    [ProducesResponseType(typeof(CommitmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommitmentDto>> UpdateCommitment(int engagementId, int commitmentId, [FromBody] UpdateCommitmentRequest request)
    {
        var dto = await commitmentService.UpdateCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return Ok(dto);
    }

    /// <summary>
    /// Deletes a specific commitment from the handover (Pre Sales only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment to delete.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{commitmentId:int}")]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Delete commitment (Pre Sales only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCommitment(int engagementId, int commitmentId)
    {
        await commitmentService.DeleteCommitmentAsync(engagementId, commitmentId);
        return NoContent();
    }

    /// <summary>
    /// Creates a realistic commitment estimation for a given commitment (Project Manager only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment.</param>
    /// <param name="request">The realistic commitment data.</param>
    /// <returns>The created realistic commitment.</returns>
    [HttpPost("{commitmentId:int}/realistic")]
    [RequireEngagementRole(EngagementRoleEnum.ProjectManager)]
    [EndpointSummary("Create realistic commitment (Project Manager only)")]
    [ProducesResponseType(typeof(RealisticCommitmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RealisticCommitmentDto>> CreateRealisticCommitment(int engagementId, int commitmentId, [FromBody] CreateRealisticCommitmentRequest request)
    {
        var dto = await realisticCommitmentService.CreateRealisticCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return CreatedAtAction(nameof(CreateRealisticCommitment), new { engagementId, commitmentId }, dto);
    }

    /// <summary>
    /// Updates an existing realistic commitment estimation (Project Manager only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment.</param>
    /// <param name="request">The updated realistic commitment data.</param>
    /// <returns>The updated realistic commitment.</returns>
    [HttpPut("{commitmentId:int}/realistic")]
    [RequireEngagementRole(EngagementRoleEnum.ProjectManager)]
    [EndpointSummary("Update realistic commitment (Project Manager only)")]
    [ProducesResponseType(typeof(RealisticCommitmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RealisticCommitmentDto>> UpdateRealisticCommitment(int engagementId, int commitmentId, [FromBody] UpdateRealisticCommitmentRequest request)
    {
        var dto = await realisticCommitmentService.UpdateRealisticCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return Ok(dto);
    }
}