using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages master checklist versions, categories, and items (Admin &amp; System Viewers).
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class MasterChecklistController(IMasterChecklistService checklistService) : ControllerBase
{
    /// <summary>
    /// Retrieves the current draft version of the master checklist (Admin only).
    /// </summary>
    /// <returns>The current draft version details.</returns>
    [HttpGet("current-draft")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Get current draft master checklist (Admin only)")]
    [ProducesResponseType(typeof(MasterChecklistVersionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentDraft()
    {
        var draftVersion = await checklistService.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return NotFound(new { message = MasterChecklistConstants.Messages.NoDraftVersion });
        }

        return Ok(draftVersion);
    }

    /// <summary>
    /// Retrieves the currently published version of the master checklist.
    /// </summary>
    /// <returns>The published master checklist version.</returns>
    [HttpGet("published")]
    [EndpointSummary("Get published master checklist")]
    [ProducesResponseType(typeof(MasterChecklistVersionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublished()
    {
        var publishedVersion = await checklistService.GetPublishedVersionAsync();
        if (publishedVersion is null)
        {
            return NotFound(new { message = MasterChecklistConstants.Messages.NoPublishedVersion });
        }

        return Ok(publishedVersion);
    }

    /// <summary>
    /// Retrieves all version history records of the master checklist (Admin only).
    /// </summary>
    /// <returns>A list of master checklist versions.</returns>
    [HttpGet("versions")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Get master checklist version history (Admin only)")]
    [ProducesResponseType(typeof(List<MasterChecklistVersionSummaryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetVersionHistory()
    {
        var versions = await checklistService.GetVersionHistoryAsync();
        return Ok(versions);
    }

    /// <summary>
    /// Retrieves a specific master checklist version by ID (Admin only).
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <returns>The requested version details.</returns>
    [HttpGet("versions/{versionId:int}")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Get master checklist version by ID (Admin only)")]
    [ProducesResponseType(typeof(MasterChecklistVersionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersionById(int versionId)
    {
        var result = await checklistService.GetVersionByIdAsync(versionId);
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Retrieves master checklist categories (Admin only).
    /// </summary>
    /// <returns>A list of master checklist categories.</returns>
    [HttpGet("categories")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Get master checklist categories (Admin only)")]
    [ProducesResponseType(typeof(List<MasterChecklistCategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await checklistService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Creates a new checklist item in the draft version (Admin only).
    /// </summary>
    /// <param name="request">The creation request payload.</param>
    /// <returns>The created master checklist item.</returns>
    [HttpPost("items")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Create a master checklist item in draft (Admin only)")]
    [ProducesResponseType(typeof(MasterChecklistItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateItem([FromBody] MasterCreateChecklistItemRequestDto request)
    {
        var result = await checklistService.CreateChecklistItemAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Updates an existing draft checklist item (Admin only).
    /// </summary>
    /// <param name="id">The checklist item ID.</param>
    /// <param name="request">The update payload.</param>
    /// <returns>The updated master checklist item.</returns>
    [HttpPut("items/{id:int}")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Update a draft master checklist item (Admin only)")]
    [ProducesResponseType(typeof(MasterChecklistItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateChecklistItemRequestDto request)
    {
        var result = await checklistService.UpdateChecklistItemAsync(id, request);
        if (!result.IsSuccess)
        {
            return result.IsNotFound
                ? NotFound(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Reorders items within the draft master checklist (Admin only).
    /// </summary>
    /// <param name="request">The reorder request payload.</param>
    /// <returns>The updated list of reordered items.</returns>
    [HttpPut("items/reorder")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Reorder draft master checklist items (Admin only)")]
    [ProducesResponseType(typeof(List<MasterChecklistItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReorderItems([FromBody] ReorderMasterChecklistItemsRequestDto request)
    {
        var result = await checklistService.ReorderChecklistItemsAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Retires an item from the draft master checklist (Admin only).
    /// </summary>
    /// <param name="id">The checklist item ID.</param>
    /// <returns>The retired item details.</returns>
    [HttpPut("items/{id:int}/retire")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Retire a draft master checklist item (Admin only)")]
    [ProducesResponseType(typeof(MasterChecklistItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetireItem(int id)
    {
        var result = await checklistService.RetireChecklistItemAsync(id);
        if (!result.IsSuccess)
        {
            return result.IsNotFound
                ? NotFound(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Publishes a draft master checklist version to make it live (Admin only).
    /// </summary>
    /// <param name="versionId">The version ID to publish.</param>
    /// <returns>The published master checklist version.</returns>
    [HttpPost("versions/{versionId:int}/publish")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    [EndpointSummary("Publish draft master checklist version (Admin only)")]
    [ProducesResponseType(typeof(MasterChecklistVersionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishVersion(int versionId)
    {
        var result = await checklistService.PublishVersionAsync(versionId);
        if (!result.IsSuccess)
        {
            return result.IsNotFound
                ? NotFound(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}
