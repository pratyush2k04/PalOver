using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MasterChecklistController(IMasterChecklistService checklistService) : ControllerBase
{

    [HttpGet("current-draft")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetCurrentDraft()
    {
        var draftVersion = await checklistService.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return NotFound(new { message = "No Draft version currently exists." });
        }

        return Ok(draftVersion);
    }

    [HttpGet("published")]
    [AuthorizeRoles(RoleEnum.ProjectManagement, RoleEnum.TechLead, RoleEnum.Leadership, RoleEnum.PreSales)]
    public async Task<IActionResult> GetPublished()
    {
        var publishedVersion = await checklistService.GetPublishedVersionAsync();
        if (publishedVersion is null)
        {
            return NotFound(new { message = "No Published version currently exists." });
        }

        return Ok(publishedVersion);
    }

    [HttpGet("versions")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetVersionHistory()
    {
        var versions = await checklistService.GetVersionHistoryAsync();
        return Ok(versions);
    }

    [HttpGet("versions/{versionId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetVersionById(int versionId)
    {
        var result = await checklistService.GetVersionByIdAsync(versionId);
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpGet("categories")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await checklistService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost("items")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateItem([FromBody] MasterCreateChecklistItemRequestDto request)
    {
        var result = await checklistService.CreateChecklistItemAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPut("items/{id}")]
    [Authorize(Policy = "AdminOnly")]
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

    [HttpPut("items/reorder")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ReorderItems([FromBody] ReorderMasterChecklistItemsRequestDto request)
    {
        var result = await checklistService.ReorderChecklistItemsAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPut("items/{id}/retire")]
    [Authorize(Policy = "AdminOnly")]
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

    [HttpPost("versions/{versionId}/publish")]
    [Authorize(Policy = "AdminOnly")]
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
