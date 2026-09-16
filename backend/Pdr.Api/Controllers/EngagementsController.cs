using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages engagements, owner assignments, and SOW status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EngagementsController(
    IEngagementService engagementService,
    IKickoffReadinessService kickoffReadinessService,
    IKickoffReminderService kickoffReminderService,
    IEngagementAccessService engagementAccessService) : ControllerBase
{
    /// <summary>
    /// Retrieves engagements assigned to the logged-in user.
    /// </summary>
    /// <param name="sortBy">Sort field ("kickoff", "created", etc.).</param>
    /// <returns>A list of assigned engagements.</returns>
    [HttpGet("my-engagements")]
    [EndpointSummary("Get my assigned engagements")]
    [ProducesResponseType(typeof(EngagementListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EngagementListResponse>> GetMyEngagements([FromQuery] string sortBy = "kickoff")
    {
        var result = await engagementService.GetMyEngagementsAsync(sortBy);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paged, filtered, sorted page of engagements assigned to the logged-in user.
    /// </summary>
    /// <param name="status">Tab filter ("Active" or "Past").</param>
    /// <param name="sortBy">Sort field ("kickoff", "readiness", or "client").</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A page of assigned engagements, plus Active/Past tab counts.</returns>
    [HttpGet("my-engagements/page")]
    [EndpointSummary("Get a paged page of my assigned engagements")]
    [ProducesResponseType(typeof(MyEngagementsPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MyEngagementsPageDto>> GetMyEngagementsPage(
        [FromQuery] string status = "Active",
        [FromQuery] string sortBy = "kickoff",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await engagementService.GetMyEngagementsPageAsync(status, sortBy, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Gets all engagements in the system.
    /// </summary>
    /// <returns>List of all engagements.</returns>
    /// <response code="200">List of all engagements returned successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EngagementInfoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EngagementInfoDto>>> GetAllEngagements()
    {
        var engagements = await engagementService.GetAllEngagementsAsync();
        return Ok(engagements);
    }

    /// <summary>
    /// Retrieves full details for a specific engagement by ID.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>Engagement information details.</returns>
    [HttpGet("{id:int}")]
    [EndpointSummary("Get engagement details by ID")]
    [ProducesResponseType(typeof(EngagementInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementInfoDto>> GetEngagementInfo(int id)
    {
        var info = await engagementService.GetEngagementInfoAsync(id);
        return info is null ? NotFound() : Ok(info);
    }

    /// <summary>
    /// Updates engagement information and schedule dates.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">The edit payload.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id:int}")]
    [EndpointSummary("Edit engagement information")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditEngagementInfo(int id, EditEngagementRequest request)
    {
        try
        {
            var success = await engagementService.EditEngagementAsync(id, request);
            return success ? NoContent() : NotFound();
        }
        catch (InvalidEngagementTypeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Marks an engagement statement of work as signed (Pre Sales only).
    /// </summary>
    /// <param name="request">The mark SOW signed request payload.</param>
    /// <returns>The updated SOW signed status response.</returns>
    [HttpPost("mark-sow-signed")]
    [AuthorizeRoles(UserRoleEnum.PreSales)]
    [EndpointSummary("Mark SOW as signed (Pre Sales only)")]
    [ProducesResponseType(typeof(MarkSowSignedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarkSowSignedResponse>> MarkSowSigned(MarkSowSignedRequest request)
    {
        var response = await engagementService.MarkSowSignedAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves current Project Manager and Tech Lead owners for an engagement (Leadership only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>Engagement owner details.</returns>
    [HttpGet("{id:int}/owners")]
    [EndpointSummary("Get engagement owners")]
    [ProducesResponseType(typeof(EngagementOwnersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementOwnersDto>> GetEngagementOwners(int id)
    {
        var owners = await engagementService.GetEngagementOwnersAsync(id);
        return Ok(owners);
    }

    /// <summary>
    /// Retrieves all system users assigned to an engagement.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>A list of users assigned to the engagement.</returns>
    [HttpGet("{id:int}/users")]
    [EndpointSummary("Get all assigned users")]
    [ProducesResponseType(typeof(IEnumerable<Pdr.Api.DTOs.Users.UserListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Pdr.Api.DTOs.Users.UserListItemDto>>> GetEngagementUsers(int id)
    {
        var users = await engagementService.GetEngagementUsersAsync(id);
        return Ok(users);
    }

    /// <summary>
    /// Assigns a Project Manager to an engagement (Leadership only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">The assigned user payload.</param>
    /// <returns>The updated engagement owners.</returns>
    [HttpPut("{id:int}/project-manager")]
    [AuthorizeRoles(UserRoleEnum.Leadership)]
    [EndpointSummary("Assign Project Manager (Leadership only)")]
    [ProducesResponseType(typeof(EngagementOwnersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementOwnersDto>> AssignProjectManager(int id, AssignOwnerRequest request)
    {
        var owners = await engagementService.AssignProjectManagerAsync(id, request.UserId);
        return Ok(owners);
    }

    /// <summary>
    /// Assigns a Tech Lead to an engagement (Leadership only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">The assigned user payload.</param>
    /// <returns>The updated engagement owners.</returns>
    [HttpPut("{id:int}/tech-lead")]
    [AuthorizeRoles(UserRoleEnum.Leadership)]
    [EndpointSummary("Assign Tech Lead (Leadership only)")]
    [ProducesResponseType(typeof(EngagementOwnersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementOwnersDto>> AssignTechLead(int id, AssignOwnerRequest request)
    {
        var owners = await engagementService.AssignTechLeadAsync(id, request.UserId);
        return Ok(owners);
    }

    [HttpGet("{id:int}/kickoff-readiness")]
    public async Task<ActionResult<KickoffReadinessDto>> GetKickoffReadiness(int id)
    {
        var readiness = await kickoffReadinessService.GetReadinessAsync(id);
        await kickoffReminderService.SyncSuppressionAsync(id, readiness.Score);
        return Ok(readiness);
    }

    [HttpGet("{id:int}/readiness-one-pager")]
    public async Task<ActionResult<ReadinessOnePagerDto>> GetReadinessOnePager(int id)
    {
        var onePager = await kickoffReadinessService.GetOnePagerAsync(id);
        return Ok(onePager);
    }

    [HttpPost("{id:int}/kickoff-ready")]
    public async Task<IActionResult> MarkKickoffReady(int id)
    {
        await engagementAccessService.RequireEngagementRoleAsync(id, EngagementRoleEnum.ProjectManager, EngagementRoleEnum.TechLead, EngagementRoleEnum.Leadership);
        var isLeadership = User.IsInRole(UserRoleEnum.Leadership.ToRoleName()) ||
                           await engagementAccessService.HasEngagementRoleAsync(id, EngagementRoleEnum.Leadership);
        await kickoffReadinessService.MarkKickoffReadyAsync(id, isLeadership);
        return NoContent();
    }

    [HttpDelete("{id:int}/kickoff-ready")]
    public async Task<IActionResult> RevertKickoffReady(int id)
    {
        await engagementAccessService.RequireEngagementRoleAsync(id, EngagementRoleEnum.Leadership);
        await kickoffReadinessService.RevertKickoffReadyAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Gets all Leadership users assigned to an engagement.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>List of assigned Leadership users.</returns>
    /// <response code="200">List of assigned Leadership users returned successfully.</response>
    /// <response code="404">Engagement was not found.</response>
    [HttpGet("{id:int}/leadership-users")]
    [ProducesResponseType(typeof(IEnumerable<LeadershipUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LeadershipUserDto>>> GetLeadershipUsers(int id)
    {
        var users = await engagementService.GetLeadershipUsersAsync(id);
        return Ok(users);
    }

    /// <summary>
    /// Gets whether an engagement has a handover draft and its content, for use in the leadership allocation flow (Admin only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>Handover draft status and content, independent of the caller's engagement assignment.</returns>
    /// <response code="200">Handover draft status returned successfully.</response>
    /// <response code="404">Engagement was not found.</response>
    [HttpGet("{id:int}/handover-draft-status")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(HandoverDraftStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HandoverDraftStatusDto>> GetHandoverDraftStatus(int id)
    {
        var status = await engagementService.GetHandoverDraftStatusAsync(id);
        return Ok(status);
    }

    /// <summary>
    /// Assigns one or more Leadership users to an engagement.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">Payload containing single UserId or list UserIds of Leadership users to assign.</param>
    /// <returns>Updated list of assigned Leadership users.</returns>
    /// <response code="200">Leadership user(s) assigned successfully.</response>
    /// <response code="400">User is already assigned to engagement or invalid role.</response>
    /// <response code="404">Engagement or User was not found.</response>
    [HttpPost("{id:int}/leadership-users")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<LeadershipUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LeadershipUserDto>>> AssignLeadershipUsers(int id, [FromBody] AssignLeadershipUserRequest request)
    {
        if (request.UserIds is not null && request.UserIds.Count > 0)
        {
            var result = await engagementService.AssignLeadershipUsersAsync(id, request.UserIds);
            return Ok(result);
        }

        var updated = await engagementService.AssignLeadershipUserAsync(id, request.UserId);
        return Ok(updated);
    }

    /// <summary>
    /// Removes a Leadership user assignment from an engagement (Admin only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="userId">The ID of the Leadership user to remove.</param>
    /// <returns>Updated list of assigned Leadership users.</returns>
    /// <response code="200">Leadership user removed successfully.</response>
    /// <response code="400">Attempted to remove the last remaining Leadership contact.</response>
    /// <response code="404">Engagement or User assignment was not found.</response>
    [HttpDelete("{id:int}/leadership-users/{userId:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<LeadershipUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LeadershipUserDto>>> RemoveLeadershipUser(int id, int userId)
    {
        var updated = await engagementService.RemoveLeadershipUserAsync(id, userId);
        return Ok(updated);
    }

    /// <summary>
    /// Replaces the entire set of Leadership users assigned to an engagement (Admin only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">Payload containing the complete list of Leadership UserIds to assign.</param>
    /// <returns>Updated list of assigned Leadership users.</returns>
    /// <response code="200">Leadership users updated successfully.</response>
    /// <response code="400">Attempted to remove the last remaining Leadership contact, or invalid request.</response>
    /// <response code="404">Engagement or User was not found.</response>
    [HttpPut("{id:int}/leadership-users")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<LeadershipUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LeadershipUserDto>>> UpdateLeadershipUsers(int id, [FromBody] UpdateLeadershipUsersRequest request)
    {
        var updated = await engagementService.UpdateLeadershipUsersAsync(id, request.UserIds);
        return Ok(updated);
    }

    /// <summary>
    /// Retrieves active engagements summary for Leadership dashboard.
    /// </summary>
    /// <returns>Leadership dashboard data with readiness score, open blocker count, days-to-kickoff, and computed status.</returns>
    [HttpGet("leadership-dashboard")]
    [AuthorizeRoles(UserRoleEnum.Leadership)]
    [EndpointSummary("Get leadership portfolio dashboard engagements")]
    [ProducesResponseType(typeof(IEnumerable<LeadershipEngagementListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<LeadershipEngagementListItemDto>>> GetLeadershipDashboard()
    {
        var dashboard = await engagementService.GetLeadershipDashboardAsync();
        return Ok(dashboard);
    }

    /// <summary>
    /// Retrieves open items summary for a specific engagement (checklist open, access open, open risks, resourcing open).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    [HttpGet("{id:int}/open-items-summary")]
    [EndpointSummary("Get open items summary")]
    [ProducesResponseType(typeof(EngagementOpenItemsSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EngagementOpenItemsSummaryDto>> GetOpenItemsSummary(int id)
    {
        var summary = await engagementService.GetOpenItemsSummaryAsync(id);
        return summary is null ? NotFound() : Ok(summary);
    }

    /// <summary>
    /// Records Day-1 readiness sign-off for an engagement against a billing date (Project Management and Leadership only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">Sign-off payload referencing the billing start date and optional leadership exception.</param>
    /// <returns>The recorded Day-1 sign-off details.</returns>
    [HttpPost("{id:int}/day-one-sign-off")]
    [EndpointSummary("Sign off Day-1 readiness for billing")]
    [ProducesResponseType(typeof(DayOneSignOffDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DayOneSignOffDto>> SignOffDayOne(int id, [FromBody] DayOneSignOffRequest request)
    {
        var result = await engagementService.SignOffDayOneAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the read-only evidence snapshot captured at Day-1 sign-off for an engagement.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>The immutable evidence snapshot, including every evidence-required checklist item's status at sign-off.</returns>
    [HttpGet("{id:int}/evidence-snapshot")]
    [EndpointSummary("Get the Day-1 sign-off evidence snapshot")]
    [ProducesResponseType(typeof(EvidenceSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EvidenceSnapshotDto>> GetEvidenceSnapshot(int id)
    {
        var result = await engagementService.GetEvidenceSnapshotAsync(id);
        return Ok(result);
    }
}
