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
[Produces("application/json")]
public class EngagementsController(
    IEngagementService engagementService,
    IKickoffReadinessService kickoffReadinessService) : ControllerBase
{
    /// <summary>
    /// Retrieves engagements assigned to the logged-in user.
    /// </summary>
    /// <param name="sortBy">Sort field ("kickoff", "created", etc.).</param>
    /// <returns>A list of assigned engagements.</returns>
    [HttpGet("my-engagements")]
    [Authorize]
    [EndpointSummary("Get my assigned engagements")]
    [ProducesResponseType(typeof(EngagementListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EngagementListResponse>> GetMyEngagements([FromQuery] string sortBy = "kickoff")
    {
        var result = await engagementService.GetMyEngagementsAsync(sortBy);
        return Ok(result);
    }

    /// <summary>
    /// Gets all engagements in the system.
    /// </summary>
    /// <returns>List of all engagements.</returns>
    /// <response code="200">List of all engagements returned successfully.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<EngagementInfoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EngagementInfoDto>>> GetAllEngagements()
    {
        var engagements = await engagementService.GetAllEngagementsAsync();
        return Ok(engagements);
    }

    //TODO: Authentication & Authorization constraints : Depends on 645

    /// <summary>
    /// Retrieves full details for a specific engagement by ID.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <returns>Engagement information details.</returns>
    [HttpGet("{id:int}")]
    [EndpointSummary("Get engagement details by ID")]
    [ProducesResponseType(typeof(EngagementInfoDto), StatusCodes.Status200OK)]
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
    [AuthorizeRoles(RoleEnum.PreSales)]
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
    [Authorize]
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
    /// Assigns a Project Manager to an engagement (Leadership only).
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <param name="request">The assigned user payload.</param>
    /// <returns>The updated engagement owners.</returns>
    [HttpPut("{id:int}/project-manager")]
    [AuthorizeRoles(RoleEnum.Leadership)]
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
    [AuthorizeRoles(RoleEnum.Leadership)]
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
    [AuthorizeRoles(RoleEnum.PreSales, RoleEnum.ProjectManagement, RoleEnum.TechLead, RoleEnum.Leadership)]
    public async Task<ActionResult<KickoffReadinessDto>> GetKickoffReadiness(int id)
    {
        var readiness = await kickoffReadinessService.GetReadinessAsync(id);
        return Ok(readiness);
    }

    [HttpPost("{id:int}/kickoff-ready")]
    [AuthorizeRoles(RoleEnum.ProjectManagement, RoleEnum.TechLead, RoleEnum.Leadership)]
    public async Task<IActionResult> MarkKickoffReady(int id)
    {
        var isLeadership = User.IsInRole(RoleEnum.Leadership.ToRoleName());
        await kickoffReadinessService.MarkKickoffReadyAsync(id, isLeadership);
        return NoContent();
    }

    [HttpDelete("{id:int}/kickoff-ready")]
    [AuthorizeRoles(RoleEnum.Leadership)]
    public async Task<IActionResult> RevertKickoffReady(int id)
    {
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
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<LeadershipUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LeadershipUserDto>>> GetLeadershipUsers(int id)
    {
        var users = await engagementService.GetLeadershipUsersAsync(id);
        return Ok(users);
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
    /// <response code="400">Attempted to remove the last remaining Leadership contact on the engagement.</response>
    /// <response code="404">Engagement or User was not found.</response>
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
    /// Retrieves active engagements summary for Leadership dashboard.
    /// </summary>
    /// <returns>Leadership dashboard data with readiness score, open blocker count, days-to-kickoff, and computed status.</returns>
    [HttpGet("leadership-dashboard")]
    [AuthorizeRoles(RoleEnum.Leadership)]
    [EndpointSummary("Get leadership portfolio dashboard engagements")]
    [ProducesResponseType(typeof(IEnumerable<LeadershipEngagementListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<LeadershipEngagementListItemDto>>> GetLeadershipDashboard()
    {
        var dashboard = await engagementService.GetLeadershipDashboardAsync();
        return Ok(dashboard);
    }
}
