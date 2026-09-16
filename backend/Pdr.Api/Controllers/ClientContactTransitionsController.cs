using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.ClientContactTransitions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages client contact transitions during engagement handovers.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/client-contact-transition")]
[Produces("application/json")]
public class ClientContactTransitionsController(IClientContactTransitionService transitionService) : ControllerBase
{
    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Retrieves the latest client contact transition for an engagement.
    /// </summary>
    [HttpGet]
    [EndpointSummary("Get latest client contact transition")]
    [ProducesResponseType(typeof(ClientContactTransitionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientContactTransitionResponseDto>> GetTransition(int engagementId)
    {
        var result = await transitionService.GetByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the complete history of client contact transitions for an engagement.
    /// </summary>
    [HttpGet("history")]
    [EndpointSummary("Get client contact transition history")]
    [ProducesResponseType(typeof(List<ClientContactTransitionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ClientContactTransitionResponseDto>>> GetHistory(int engagementId)
    {
        var result = await transitionService.GetAllByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Records a new client contact transition for an engagement (Pre Sales only).
    /// </summary>
    [HttpPost]
    [EndpointSummary("Record client contact transition")]
    [ProducesResponseType(typeof(ClientContactTransitionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientContactTransitionResponseDto>> SaveTransition(
        int engagementId,
        [FromBody] SaveClientContactTransitionRequest request)
    {
        var result = await transitionService.SaveTransitionAsync(engagementId, request, GetUserId());
        return Ok(result);
    }
}
