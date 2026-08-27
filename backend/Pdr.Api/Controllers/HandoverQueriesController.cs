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
/// Manages PM-raised queries against a submitted handover and Pre Sales responses to them.
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
    private readonly IValidator<RespondHandoverQueryRequest> _respondValidator;

    public HandoverQueriesController(
        IHandoverQueriesService handoverQueriesService,
        IHandoverAccessService handoverAccessService,
        ICurrentUserService currentUserService,
        IValidator<CreateHandoverQueryRequest> createValidator,
        IValidator<RespondHandoverQueryRequest> respondValidator)
    {
        _handoverQueriesService = handoverQueriesService;
        _handoverAccessService = handoverAccessService;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _respondValidator = respondValidator;
    }

    /// <summary>
    /// Retrieves all queries raised against this engagement's handover.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <returns>The list of handover queries.</returns>
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
    /// Raises a new query against a field of the submitted handover (Project Manager only).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="request">The query to raise.</param>
    /// <returns>The created query.</returns>
    [HttpPost]
    [EndpointSummary("Raise a handover query (Project Manager only)")]
    [ProducesResponseType(typeof(HandoverQueryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverQueryDto>> RaiseQuery(int engagementId, [FromBody] CreateHandoverQueryRequest request)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId(), RoleConstants.ProjectManagement);

        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? HandoverQueryConstants.InvalidRequest;
            return BadRequest(new { message = firstError });
        }

        var dto = await _handoverQueriesService.RaiseQueryAsync(engagementId, request, _currentUserService.GetUserId().ToString());
        return CreatedAtAction(nameof(GetQueries), new { engagementId }, dto);
    }

    /// <summary>
    /// Responds to a handover query (Pre Sales only). A query can only be responded to once.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="queryId">The unique identifier of the query.</param>
    /// <param name="request">The response to record.</param>
    /// <returns>The updated query.</returns>
    [HttpPut("{queryId:int}/respond")]
    [EndpointSummary("Respond to a handover query (Pre Sales only)")]
    [ProducesResponseType(typeof(HandoverQueryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HandoverQueryDto>> RespondToQuery(int engagementId, int queryId, [FromBody] RespondHandoverQueryRequest request)
    {
        await _handoverAccessService.CanAccessHandoverAsync(engagementId, _currentUserService.GetUserId(), RoleConstants.PreSales);

        var validationResult = await _respondValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? HandoverQueryConstants.InvalidResponse;
            return BadRequest(new { message = firstError });
        }

        var dto = await _handoverQueriesService.RespondToQueryAsync(engagementId, queryId, request, _currentUserService.GetUserId().ToString());
        return Ok(dto);
    }

    /// <summary>
    /// Toggles a handover query between resolved and reopened (Pre Sales or Project Manager).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="queryId">The unique identifier of the query.</param>
    /// <returns>The updated query.</returns>
    [HttpPut("{queryId:int}/toggle-resolution")]
    [EndpointSummary("Toggle resolved/reopened state of a handover query (Pre Sales or Project Manager)")]
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
}
