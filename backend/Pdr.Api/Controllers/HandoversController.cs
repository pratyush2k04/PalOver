using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages sales-to-delivery handovers, draft saving, and final submission.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/handover")]
[Produces("application/json")]
public class HandoversController : ControllerBase
{
    private readonly IHandoverService _handoverService;
    private readonly IHandoverAccessService _accessService;

    public HandoversController(
        IHandoverService handoverService,
        IHandoverAccessService accessService)
    {
        _handoverService = handoverService;
        _accessService = accessService;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private string GetUserIdString() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

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
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var dto = await _handoverService.GetHandoverAsync(engagementId);
        return Ok(dto);
    }

    /// <summary>
    /// Saves or updates the handover details as a draft.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="request">The draft data to save.</param>
    /// <returns>The updated handover details.</returns>
    [HttpPut("draft")]
    [EndpointSummary("Save handover draft")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> SaveDraft(int engagementId, [FromBody] SaveHandoverDraftRequest request)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId(), RoleConstants.PreSales);

        var dto = await _handoverService.SaveHandoverDraftAsync(engagementId, request, GetUserIdString());
        return Ok(dto);
    }

    /// <summary>
    /// Submits the handover, finalizing its state and locking it from further edits.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The submitted handover details.</returns>
    [HttpPost("submit")]
    [EndpointSummary("Submit handover (finalizing state)")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> Submit(int engagementId)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId(), RoleConstants.PreSales);

        var dto = await _handoverService.SubmitHandoverAsync(engagementId, GetUserIdString());
        return Ok(dto);
    }
}
