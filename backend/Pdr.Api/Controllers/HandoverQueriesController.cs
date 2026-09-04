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
/// Manages PM-raised queries and conversational discussion turns between Project Management and Pre Sales on a submitted handover.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/handover/queries")]
[Produces("application/json")]
public class HandoverQueriesController : ControllerBase
{
    private readonly IHandoverQueriesService _handoverQueriesService;
    private readonly IHandoverAccessService _handoverAccessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateHandoverQueryRequest> _createValidator;
    private readonly IValidator<AddHandoverQueryMessageRequest> _messageValidator;

    public HandoverQueriesController(
        IHandoverQueriesService handoverQueriesService,
        IHandoverAccessService handoverAccessService,
        ICurrentUserService currentUserService,
        IValidator<CreateHandoverQueryRequest> createValidator,
        IValidator<AddHandoverQueryMessageRequest> messageValidator)
    {
        _handoverQueriesService = handoverQueriesService;
        _handoverAccessService = handoverAccessService;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _messageValidator = messageValidator;
    }

    /// <summary>
    /// Retrieves all queries raised against this engagement's handover with full conversational message histories.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The list of handover queries with message history.</returns>
    [HttpGet]
    [EndpointSummary("Get handover queries")]
    [ProducesResponseType(typeof(List<HandoverQueryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HandoverQueryDto>>> GetQueries(int engagementId)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId());

        var dtos = await _handoverQueriesService.GetQueriesAsync(engagementId);
        return Ok(dtos);
    }

    /// <summary>
    /// Raises a new query thread against a field section of the submitted handover (Project Management only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="request">The query to raise.</param>
    /// <returns>The created query thread with initial question.</returns>
    [HttpPost]
    [EndpointSummary("Raise a handover query (Project Management only)")]
    [ProducesResponseType(typeof(HandoverQueryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverQueryDto>> RaiseQuery(int engagementId, [FromBody] CreateHandoverQueryRequest request)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId(), RoleConstants.ProjectManagement);

        await _createValidator.ValidateAndThrowAsync(request);

        var dto = await _handoverQueriesService.RaiseQueryAsync(engagementId, request, _currentUserService.GetUserId().ToString());
        return CreatedAtAction(nameof(GetQueries), new { engagementId }, dto);
    }

    /// <summary>
    /// Adds a conversational message turn to a handover query (Pre Sales response or PM follow-up question).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="queryId">The unique identifier of the query.</param>
    /// <param name="request">The message text payload.</param>
    /// <returns>The updated query with new message in history.</returns>
    [HttpPost("{queryId:int}/messages")]
    [EndpointSummary("Add a message turn to a handover query (Pre Sales response or PM follow-up)")]
    [ProducesResponseType(typeof(HandoverQueryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HandoverQueryDto>> AddMessage(int engagementId, int queryId, [FromBody] AddHandoverQueryMessageRequest request)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId(), RoleConstants.PreSales, RoleConstants.ProjectManagement);

        await _messageValidator.ValidateAndThrowAsync(request);

        var dto = await _handoverQueriesService.AddMessageAsync(engagementId, queryId, request, _currentUserService.GetUserId().ToString());
        return Ok(dto);
    }

    /// <summary>
    /// Toggles a handover query between resolved and reopened (Pre Sales or Project Management).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="queryId">The unique identifier of the query.</param>
    /// <returns>The updated query.</returns>
    [HttpPut("{queryId:int}/toggle-resolution")]
    [EndpointSummary("Toggle resolved/reopened state of a handover query (Pre Sales or Project Management)")]
    [ProducesResponseType(typeof(HandoverQueryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HandoverQueryDto>> ToggleResolution(int engagementId, int queryId)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId(), RoleConstants.PreSales, RoleConstants.ProjectManagement);

        var dto = await _handoverQueriesService.ToggleResolutionAsync(engagementId, queryId, _currentUserService.GetUserId().ToString());
        return Ok(dto);
    }

    /// <summary>
    /// Deletes a handover query (Project Management only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="queryId">The unique identifier of the query.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{queryId:int}")]
    [EndpointSummary("Delete a handover query (Project Management only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuery(int engagementId, int queryId)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId(), RoleConstants.ProjectManagement);

        await _handoverQueriesService.DeleteQueryAsync(engagementId, queryId, _currentUserService.GetUserId().ToString());
        return NoContent();
    }
}
