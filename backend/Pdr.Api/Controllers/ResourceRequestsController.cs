using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.ResourceRequests;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages structured resource requests for an engagement.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/resource-requests")]
[Produces("application/json")]
public class ResourceRequestsController(IResourceRequestService resourceRequestService) : ControllerBase
{
    /// <summary>
    /// Retrieves all resource requests for a given engagement.
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <returns>A list of resource requests including calculated urgency status.</returns>
    [HttpGet]
    [EndpointSummary("Get resource requests for an engagement")]
    [ProducesResponseType(typeof(List<ResourceRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ResourceRequestDto>>> GetResourceRequests(int engagementId)
    {
        var result = await resourceRequestService.GetForEngagementAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Raises a new structured resource request for an engagement in "Raised" state (Project Manager assigned to the engagement only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="request">The resource request details including role, skills, headcount, required-by date, and job description.</param>
    /// <returns>The created resource request details.</returns>
    [HttpPost]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    [EndpointSummary("Raise a structured resource request (Project Manager only)")]
    [ProducesResponseType(typeof(ResourceRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceRequestDto>> CreateResourceRequest(
        int engagementId,
        [FromBody] CreateResourceRequestDto request)
    {
        var result = await resourceRequestService.CreateResourceRequestAsync(engagementId, request);
        return CreatedAtAction(nameof(GetResourceRequests), new { engagementId }, result);
    }
}
