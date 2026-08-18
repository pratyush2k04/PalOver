using System.Security.Claims;
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
    private readonly IHandoverQueryService _handoverQueryService;
    private readonly IHandoverAccessService _accessService;
    private readonly IValidator<CreateHandoverQueryRequest> _createValidator;
    private readonly IValidator<RespondHandoverQueryRequest> _respondValidator;

    public HandoverQueriesController(
        IHandoverQueryService handoverQueryService,
        IHandoverAccessService accessService,
        IValidator<CreateHandoverQueryRequest> createValidator,
        IValidator<RespondHandoverQueryRequest> respondValidator)
    {
        _handoverQueryService = handoverQueryService;
        _accessService = accessService;
        _createValidator = createValidator;
        _respondValidator = respondValidator;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private string GetUserIdString() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

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
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var dtos = await _handoverQueryService.GetQueriesAsync(engagementId);
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
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId(), RoleConstants.ProjectManagement);

        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid handover query request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _handoverQueryService.RaiseQueryAsync(engagementId, request, GetUserIdString());
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
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId(), RoleConstants.PreSales);

        var validationResult = await _respondValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid response.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _handoverQueryService.RespondToQueryAsync(engagementId, queryId, request, GetUserIdString());
        return Ok(dto);
    }

    /// <summary>
    /// Marks a handover query resolved or reopens it (Pre Sales or Project Manager).
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="queryId">The unique identifier of the query.</param>
    /// <param name="request">The resolved state to set.</param>
    /// <returns>The updated query.</returns>
    [HttpPut("{queryId:int}/resolve")]
    [EndpointSummary("Resolve or reopen a handover query (Pre Sales or Project Manager)")]
    [ProducesResponseType(typeof(HandoverQueryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverQueryDto>> ResolveQuery(int engagementId, int queryId, [FromBody] ResolveHandoverQueryRequest request)
    {
        await _accessService.CanAccessHandoverAsync(engagementId, GetUserId());

        var dto = await _handoverQueryService.ResolveQueryAsync(engagementId, queryId, request, GetUserIdString());
        return Ok(dto);
    }
}
