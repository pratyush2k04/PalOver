using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/engagements/{engagementId:int}/handover")]
[Produces("application/json")]
public class HandoversController : ControllerBase
{
    private readonly IHandoverService _handoverService;
    private readonly IHandoverAccessService _accessService;
    private readonly IValidator<SaveHandoverDraftRequest> _saveDraftValidator;

    public HandoversController(
        IHandoverService handoverService,
        IHandoverAccessService accessService,
        IValidator<SaveHandoverDraftRequest> saveDraftValidator)
    {
        _handoverService = handoverService;
        _accessService = accessService;
        _saveDraftValidator = saveDraftValidator;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private string GetUserIdString() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;


    /// <summary>
    /// Retrieves the handover information and associated commitments for a specific engagement.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The handover details.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> SaveDraft(int engagementId, [FromBody] SaveHandoverDraftRequest request)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var validationResult = await _saveDraftValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid handover draft request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _handoverService.SaveHandoverDraftAsync(engagementId, request, GetUserIdString());
        return Ok(dto);
    }

    /// <summary>
    /// Submits the handover, finalizing its state and locking it from further edits.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The submitted handover details.</returns>
    [HttpPost("submit")]
    [ProducesResponseType(typeof(HandoverInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverInfoDto>> Submit(int engagementId)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var dto = await _handoverService.SubmitHandoverAsync(engagementId, GetUserIdString());
        return Ok(dto);
    }
}
