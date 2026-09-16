using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.CommunicationCadences;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages governance communication cadences for an engagement.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/cadences")]
[Produces("application/json")]
public class CommunicationCadencesController : ControllerBase
{
    private readonly ICommunicationCadenceService _cadenceService;

    public CommunicationCadencesController(ICommunicationCadenceService cadenceService)
    {
        _cadenceService = cadenceService;
    }

    /// <summary>
    /// Retrieves all communication cadences configured for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>A list of communication cadences.</returns>
    [HttpGet]
    [EndpointSummary("Get all communication cadences for an engagement")]
    [ProducesResponseType(typeof(List<CommunicationCadenceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CommunicationCadenceResponseDto>>> GetCadences(int engagementId)
    {
        var result = await _cadenceService.GetCadencesAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves team members assigned to an engagement for dropdown selection.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>A list of assigned team participants.</returns>
    [HttpGet("assigned-participants")]
    [EndpointSummary("Get team members assigned to an engagement for dropdown selection")]
    [ProducesResponseType(typeof(List<EngagementParticipantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<EngagementParticipantDto>>> GetAssignedParticipants(int engagementId)
    {
        var result = await _cadenceService.GetAssignedParticipantsAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new communication cadence for a stakeholder role (Project Manager only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The cadence creation request payload.</param>
    /// <returns>The created communication cadence.</returns>
    [HttpPost]
    [EndpointSummary("Create a communication cadence (Project Manager only)")]
    [ProducesResponseType(typeof(CommunicationCadenceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommunicationCadenceResponseDto>> AddCadence(
        int engagementId,
        [FromBody] CreateCommunicationCadenceRequest request)
    {
        var result = await _cadenceService.AddCadenceAsync(engagementId, request);
        return CreatedAtAction(nameof(GetCadences), new { engagementId }, result);
    }
}
