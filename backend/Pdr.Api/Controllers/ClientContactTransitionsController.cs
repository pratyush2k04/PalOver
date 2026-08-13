using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.ClientContactTransitions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/client-contact-transition")]
public class ClientContactTransitionsController(
    IClientContactTransitionService transitionService,
    IValidator<SaveClientContactTransitionRequest> validator) : ControllerBase
{
    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<ActionResult<ClientContactTransitionResponseDto>> GetTransition(int engagementId)
    {
        var result = await transitionService.GetByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<ClientContactTransitionResponseDto>>> GetHistory(int engagementId)
    {
        var result = await transitionService.GetAllByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Pre Sales")]
    public async Task<ActionResult<ClientContactTransitionResponseDto>> SaveTransition(
        int engagementId,
        [FromBody] SaveClientContactTransitionRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var result = await transitionService.SaveTransitionAsync(engagementId, request, GetUserId());
        return Ok(result);
    }
}
