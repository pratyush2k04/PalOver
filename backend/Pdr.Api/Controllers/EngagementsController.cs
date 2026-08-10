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
public class EngagementsController(IEngagementService engagementService) : ControllerBase
{
    /// <summary>
    /// Retrieves engagements assigned to the logged-in Project Manager or Tech Lead.
    /// </summary>
    /// <param name="sortBy">Sort field ("kickoff", "created", etc.).</param>
    /// <returns>A list of assigned engagements.</returns>
    [HttpGet("my-engagements")]
    [AuthorizeRoles(RoleEnum.ProjectManagement, RoleEnum.TechLead)]
    [EndpointSummary("Get my assigned engagements")]
    [ProducesResponseType(typeof(PmEngagementListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PmEngagementListResponse>> GetMyEngagements([FromQuery] string sortBy = "kickoff")
    {
        var result = await engagementService.GetMyEngagementsAsync(sortBy);
        return Ok(result);
    }

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
    [AuthorizeRoles(RoleEnum.Leadership)]
    [EndpointSummary("Get engagement owners (Leadership only)")]
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
}
