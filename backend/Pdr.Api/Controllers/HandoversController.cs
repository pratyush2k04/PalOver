using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages sales-to-delivery handovers, draft saving, and final submission.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/handover")]
[Authorize]
[RequireEngagementAccess]
[Produces("application/json")]
public class HandoversController(
    IHandoverService handoverService,
    ICurrentUserService currentUserService) : ControllerBase
{
    private string GetUserIdString() => currentUserService.GetUserId().ToString();

    /// <summary>
    /// Retrieves handover details and commitments for a specific engagement.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The handover details.</returns>
    [HttpGet]
    [EndpointSummary("Get handover details")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> GetHandover(int engagementId)
    {
        var dto = await handoverService.GetHandoverAsync(engagementId);
        return Ok(dto);
    }

    /// <summary>
    /// Saves or updates the handover details as a draft.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="request">The draft data to save.</param>
    /// <returns>The updated handover details.</returns>
    [HttpPut("draft")]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Save handover draft")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> SaveDraft(int engagementId, [FromBody] SaveHandoverDraftRequest request)
    {
        var dto = await handoverService.SaveHandoverDraftAsync(engagementId, request, GetUserIdString());
        return Ok(dto);
    }

    /// <summary>
    /// Submits the handover, finalizing its state and locking it from further edits.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The submitted handover details.</returns>
    [HttpPost("submit")]
    [RequireEngagementRole(EngagementRoleEnum.PreSales)]
    [EndpointSummary("Submit handover (finalizing state)")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> Submit(int engagementId)
    {
        var dto = await handoverService.SubmitHandoverAsync(engagementId, GetUserIdString());
        return Ok(dto);
    }

    /// <summary>
    /// Marks a submitted handover as reviewed by the Project Manager. Idempotent once reviewed.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The handover details.</returns>
    [HttpPost("review")]
    [RequireEngagementRole(EngagementRoleEnum.ProjectManager)]
    [EndpointSummary("Mark handover reviewed (Project Manager only)")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> MarkReviewed(int engagementId)
    {
        var dto = await handoverService.MarkReviewedAsync(engagementId, GetUserIdString());
        return Ok(dto);
    }
}
