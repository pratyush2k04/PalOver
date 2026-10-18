using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.DTOs.Assumptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages engagement assumptions register.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/workspace/assumptions")]
[Authorize]
[RequireEngagementAccess]
[Produces("application/json")]
public class AssumptionController(IAssumptionService assumptionService) : ControllerBase
{
    /// <summary>
    /// Retrieves all assumptions for an engagement.
    /// </summary>
    [HttpGet]
    [EndpointSummary("Get engagement assumptions")]
    [ProducesResponseType(typeof(List<AssumptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AssumptionDto>>> GetAssumptions(int engagementId)
    {
        var assumptions = await assumptionService.GetAssumptionsAsync(engagementId);
        return Ok(assumptions);
    }

    /// <summary>
    /// Retrieves a specific assumption by ID.
    /// </summary>
    [HttpGet("{assumptionId:int}")]
    [EndpointSummary("Get assumption details")]
    [ProducesResponseType(typeof(AssumptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssumptionDto>> GetAssumption(int engagementId, int assumptionId)
    {
        var assumption = await assumptionService.GetAssumptionAsync(engagementId, assumptionId);
        return Ok(assumption);
    }

    /// <summary>
    /// Creates a new assumption.
    /// </summary>
    [HttpPost]
    [EndpointSummary("Create new assumption")]
    [ProducesResponseType(typeof(AssumptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssumptionDto>> CreateAssumption(int engagementId, CreateAssumptionDto createDto)
    {
        var assumption = await assumptionService.CreateAssumptionAsync(engagementId, createDto);
        return CreatedAtAction(nameof(GetAssumption), new { engagementId, assumptionId = assumption.Id }, assumption);
    }

    /// <summary>
    /// Updates an existing assumption. Requires PM or Owner permissions.
    /// </summary>
    [HttpPut("{assumptionId:int}")]
    [EndpointSummary("Update assumption details")]
    [ProducesResponseType(typeof(AssumptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssumptionDto>> UpdateAssumption(int engagementId, int assumptionId, UpdateAssumptionDto updateDto)
    {
        var assumption = await assumptionService.UpdateAssumptionAsync(engagementId, assumptionId, updateDto);
        return Ok(assumption);
    }

    /// <summary>
    /// Validates or invalidates an assumption. Requires PM permissions.
    /// </summary>
    [HttpPut("{assumptionId:int}/validate")]
    [RequireEngagementRole(EngagementRoleEnum.ProjectManager)]
    [EndpointSummary("Validate assumption")]
    [ProducesResponseType(typeof(AssumptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssumptionDto>> ValidateAssumption(int engagementId, int assumptionId, ValidateAssumptionDto validateDto)
    {
        var assumption = await assumptionService.ValidateAssumptionAsync(engagementId, assumptionId, validateDto);
        return Ok(assumption);
    }
}
