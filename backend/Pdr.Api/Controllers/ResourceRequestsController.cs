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
public class ResourceRequestsController(
    IResourceRequestService resourceRequestService,
    IRiskService riskService) : ControllerBase
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
    /// Evaluates whether an engagement has an active resourcing gap before billing starts.
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <returns>The resourcing gap status, unconfirmed count, and days to billing.</returns>
    [HttpGet("gap")]
    [EndpointSummary("Get resourcing gap status for an engagement")]
    [ProducesResponseType(typeof(ResourcingGapDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourcingGapDto>> GetResourcingGap(int engagementId)
    {
        var result = await resourceRequestService.GetResourcingGapAsync(engagementId);
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
        await riskService.SyncAutoCreatedRisksAsync(engagementId);
        return CreatedAtAction(nameof(GetResourceRequests), new { engagementId }, result);
    }

    /// <summary>
    /// Allocates named resources against a request's open slots (Leadership assigned to the engagement, or Admin, only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="id">The ID of the resource request.</param>
    /// <param name="request">The names to allocate — one per person needed.</param>
    /// <returns>The updated resource request, including its named allocations.</returns>
    [HttpPost("{id:int}/allocations")]
    [EndpointSummary("Allocate named resources to a request (Leadership or Admin only)")]
    [ProducesResponseType(typeof(ResourceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceRequestDto>> AllocateResources(
        int engagementId,
        int id,
        [FromBody] AllocateResourcesDto request)
    {
        var result = await resourceRequestService.AllocateResourcesAsync(engagementId, id, request);
        return Ok(result);
    }

    /// <summary>
    /// Records a confirmation date for a named resource (Leadership assigned to the engagement, or Admin, only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="id">The ID of the resource request.</param>
    /// <param name="allocationId">The ID of the named resource allocation.</param>
    /// <param name="request">The confirmation date (must not be in the future).</param>
    /// <returns>The updated resource request; flips to Confirmed once every named resource is confirmed.</returns>
    [HttpPost("{id:int}/allocations/{allocationId:int}/confirm")]
    [EndpointSummary("Confirm a named resource with a confirmation date (Leadership or Admin only)")]
    [ProducesResponseType(typeof(ResourceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceRequestDto>> ConfirmAllocation(
        int engagementId,
        int id,
        int allocationId,
        [FromBody] ConfirmAllocationDto request)
    {
        var result = await resourceRequestService.ConfirmAllocationAsync(engagementId, id, allocationId, request);
        await riskService.SyncAutoCreatedRisksAsync(engagementId);
        return Ok(result);
    }

    /// <summary>
    /// Removes a confirmed named resource with a required reason, reopening the request to Allocated (Leadership assigned to the engagement, or Admin, only).
    /// </summary>
    /// <param name="engagementId">The ID of the engagement.</param>
    /// <param name="id">The ID of the resource request.</param>
    /// <param name="allocationId">The ID of the named resource allocation.</param>
    /// <param name="request">The reason for removal.</param>
    /// <returns>The updated resource request.</returns>
    [HttpPost("{id:int}/allocations/{allocationId:int}/remove")]
    [EndpointSummary("Remove a confirmed resource with a reason, reopening the request (Leadership or Admin only)")]
    [ProducesResponseType(typeof(ResourceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceRequestDto>> RemoveAllocation(
        int engagementId,
        int id,
        int allocationId,
        [FromBody] RemoveAllocationDto request)
    {
        var result = await resourceRequestService.RemoveAllocationAsync(engagementId, id, allocationId, request);
        await riskService.SyncAutoCreatedRisksAsync(engagementId);
        return Ok(result);
    }
}
