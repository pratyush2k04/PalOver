using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.ChangeRequests;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/change-requests")]
[Produces("application/json")]
public class ChangeRequestsController(IChangeRequestService changeRequestService) : ControllerBase
{
    /// <summary>
    /// Retrieves change requests for a given engagement with optional status filtering.
    /// Each rejected item includes its rejection reason directly for UI tooltip/hover display.
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="status">Optional status filter ("Under Review", "Approved", "Rejected").</param>
    /// <returns>A list of change requests including rejection reasons for rejected items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ChangeRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ChangeRequestDto>>> GetChangeRequests(
        int engagementId, 
        [FromQuery] string? status = null)
    {
        var result = await changeRequestService.GetChangeRequestsAsync(engagementId, status);
        return Ok(result);
    }

    /// <summary>
    /// Logs a new change request for an engagement in "Under Review" state (Project Management role only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="request">The description and impact estimate for the change request.</param>
    /// <returns>The created change request details.</returns>
    [HttpPost]
    [Authorize(Roles = "Project Management")]
    [ProducesResponseType(typeof(ChangeRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChangeRequestDto>> CreateChangeRequest(
        int engagementId, 
        [FromBody] CreateChangeRequestDto request)
    {
        var result = await changeRequestService.CreateChangeRequestAsync(engagementId, request);
        return CreatedAtAction(nameof(GetChangeRequests), new { engagementId }, result);
    }
}

[ApiController]
[Authorize]
[Route("api/change-requests")]
[Produces("application/json")]
public class ChangeRequestItemController(IChangeRequestService changeRequestService) : ControllerBase
{
    /// <summary>
    /// Approves a change request in "Under Review" status (Leadership role only).
    /// </summary>
    /// <param name="id">The change request ID.</param>
    /// <returns>The updated change request in Approved status.</returns>
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Leadership")]
    [ProducesResponseType(typeof(ChangeRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChangeRequestDto>> ApproveChangeRequest(int id)
    {
        var result = await changeRequestService.ApproveChangeRequestAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a change request in "Under Review" status with a required reason (Leadership role only).
    /// </summary>
    /// <param name="id">The change request ID.</param>
    /// <param name="request">The rejection payload containing the reason.</param>
    /// <returns>The updated change request in Rejected status with the recorded rejection reason.</returns>
    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Leadership")]
    [ProducesResponseType(typeof(ChangeRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChangeRequestDto>> RejectChangeRequest(
        int id, 
        [FromBody] RejectChangeRequestDto request)
    {
        var result = await changeRequestService.RejectChangeRequestAsync(id, request);
        return Ok(result);
    }
}
