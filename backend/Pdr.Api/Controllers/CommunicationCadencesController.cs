using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.CommunicationCadences;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/cadences")]
public class CommunicationCadencesController : ControllerBase
{
    private readonly ICommunicationCadenceService _cadenceService;

    public CommunicationCadencesController(ICommunicationCadenceService cadenceService)
    {
        _cadenceService = cadenceService;
    }

    [HttpGet]
    [EndpointSummary("Get all communication cadences for an engagement")]
    [ProducesResponseType(typeof(List<CommunicationCadenceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CommunicationCadenceResponseDto>>> GetCadences(int engagementId)
    {
        var result = await _cadenceService.GetCadencesAsync(engagementId);
        return Ok(result);
    }

    [HttpGet("assigned-participants")]
    [EndpointSummary("Get team members assigned to an engagement for dropdown selection")]
    [ProducesResponseType(typeof(List<EngagementParticipantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EngagementParticipantDto>>> GetAssignedParticipants(int engagementId)
    {
        var result = await _cadenceService.GetAssignedParticipantsAsync(engagementId);
        return Ok(result);
    }

    [HttpPost]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    [EndpointSummary("Create a communication cadence for a stakeholder role (Project Manager only)")]
    [ProducesResponseType(typeof(CommunicationCadenceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CommunicationCadenceResponseDto>> AddCadence(
        int engagementId,
        [FromBody] CreateCommunicationCadenceRequest request)
    {
        var result = await _cadenceService.AddCadenceAsync(engagementId, request);
        return CreatedAtAction(nameof(GetCadences), new { engagementId }, result);
    }
}
