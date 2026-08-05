using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.CommunicationCadences;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/cadences")]
public class CommunicationCadencesController : ControllerBase
{
    private readonly ICommunicationCadenceService _cadenceService;
    private readonly IValidator<CreateCommunicationCadenceRequest> _createValidator;

    public CommunicationCadencesController(
        ICommunicationCadenceService cadenceService,
        IValidator<CreateCommunicationCadenceRequest> createValidator)
    {
        _cadenceService = cadenceService;
        _createValidator = createValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommunicationCadenceResponseDto>>> GetCadences(int engagementId)
    {
        var result = await _cadenceService.GetCadencesAsync(engagementId);
        return Ok(result);
    }

    [HttpGet("assigned-participants")]
    public async Task<ActionResult<List<EngagementParticipantDto>>> GetAssignedParticipants(int engagementId)
    {
        var result = await _cadenceService.GetAssignedParticipantsAsync(engagementId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Project Management")]
    public async Task<ActionResult<CommunicationCadenceResponseDto>> AddCadence(
        int engagementId,
        [FromBody] CreateCommunicationCadenceRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid communication cadence request.";
            return BadRequest(new { message = firstError });
        }

        var result = await _cadenceService.AddCadenceAsync(engagementId, request);
        return CreatedAtAction(nameof(GetCadences), new { engagementId }, result);
    }
}
