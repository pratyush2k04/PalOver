using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages risk discussion comments, @mentions, and edit windows.
/// </summary>
[ApiController]
[Route("api/engagements/{engagementId:int}/workspace/risks")]
[Authorize]
[Produces("application/json")]
public class RiskCommentsController(
    IRiskCommentService commentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Retrieves all discussion comments for a specific risk, ordered oldest first.
    /// </summary>
    [HttpGet("{riskId:int}/comments")]
    [EndpointSummary("Get risk comments")]
    [ProducesResponseType(typeof(IEnumerable<RiskCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RiskCommentDto>>> GetComments(int engagementId, int riskId)
    {
        var currentUserId = currentUserService.GetUserId();
        var comments = await commentService.GetCommentsAsync(engagementId, riskId, currentUserId);
        return Ok(comments);
    }

    /// <summary>
    /// Adds a new comment to a risk with optional @mentions.
    /// </summary>
    [HttpPost("{riskId:int}/comments")]
    [EndpointSummary("Create risk comment")]
    [ProducesResponseType(typeof(RiskCommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskCommentDto>> CreateComment(
        int engagementId,
        int riskId,
        [FromBody] CreateRiskCommentDto dto)
    {
        var authorId = currentUserService.GetUserId();
        var result = await commentService.CreateCommentAsync(engagementId, riskId, dto, authorId);
        return CreatedAtAction(nameof(GetComments), new { engagementId, riskId }, result);
    }

    /// <summary>
    /// Updates an existing comment (author only, within 15 minutes of creation).
    /// </summary>
    [HttpPut("{riskId:int}/comments/{commentId:int}")]
    [EndpointSummary("Update risk comment")]
    [ProducesResponseType(typeof(RiskCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskCommentDto>> UpdateComment(
        int engagementId,
        int riskId,
        int commentId,
        [FromBody] UpdateRiskCommentDto dto)
    {
        var currentUserId = currentUserService.GetUserId();
        var result = await commentService.UpdateCommentAsync(engagementId, riskId, commentId, dto, currentUserId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves members assigned to the engagement available for @mentions.
    /// </summary>
    [HttpGet("members")]
    [EndpointSummary("Get engagement members for mentions")]
    [ProducesResponseType(typeof(IEnumerable<EngagementMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<EngagementMemberDto>>> GetEngagementMembers(int engagementId)
    {
        var members = await commentService.GetEngagementMembersAsync(engagementId);
        return Ok(members);
    }
}
