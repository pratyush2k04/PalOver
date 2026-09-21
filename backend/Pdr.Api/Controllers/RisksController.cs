using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Models.Risks;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages engagement risks register and risk resolution notes.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/workspace/risks")]
[Authorize]
[RequireEngagementAccess]
[Produces("application/json")]
public class RisksController(IRiskService riskService) : ControllerBase
{
    /// <summary>
    /// Retrieves risks for an engagement sorted by combined rating.
    /// </summary>
    [HttpGet]
    [EndpointSummary("Get engagement risk register")]
    [ProducesResponseType(typeof(IEnumerable<RiskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RiskDto>>> GetRisks(
        int engagementId,
        [FromQuery] RiskStatus? status = null,
        [FromQuery] int? ownerId = null)
    {
        var result = await riskService.GetRisksByEngagementAsync(engagementId, status, ownerId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new risk entry.
    /// </summary>
    [HttpPost]
    [EndpointSummary("Create new risk")]
    [ProducesResponseType(typeof(RiskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskDto>> CreateRisk(int engagementId, [FromBody] CreateRiskDto dto)
    {
        var result = await riskService.CreateRiskAsync(engagementId, dto);
        return CreatedAtAction(nameof(GetRisks), new { engagementId }, result);
    }

    /// <summary>
    /// Updates an existing risk entry.
    /// </summary>
    [HttpPut("{riskId:int}")]
    [EndpointSummary("Update risk details")]
    [ProducesResponseType(typeof(RiskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskDto>> UpdateRisk(int engagementId, int riskId, [FromBody] UpdateRiskDto dto)
    {
        var result = await riskService.UpdateRiskAsync(engagementId, riskId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Closes a risk entry with a mandatory resolution note (PM or Risk Owner only).
    /// </summary>
    [HttpPost("{riskId:int}/close")]
    [EndpointSummary("Close risk with resolution note")]
    [ProducesResponseType(typeof(RiskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskDto>> CloseRisk(int engagementId, int riskId, [FromBody] CloseRiskDto dto)
    {
        var result = await riskService.CloseRiskAsync(engagementId, riskId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Reopens a closed risk entry.
    /// </summary>
    [HttpPost("{riskId:int}/reopen")]
    [EndpointSummary("Reopen closed risk")]
    [ProducesResponseType(typeof(RiskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskDto>> ReopenRisk(int engagementId, int riskId)
    {
        var result = await riskService.ReopenRiskAsync(engagementId, riskId);
        return Ok(result);
    }
}
