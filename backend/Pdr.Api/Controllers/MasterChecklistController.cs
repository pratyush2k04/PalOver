using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Services.Interfaces;

using Pdr.Api.Constants;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route(MasterChecklistConstants.Routes.Base)]
public class MasterChecklistController(
    IMasterChecklistService checklistService,
    IValidator<MasterCreateChecklistItemRequestDto> createItemValidator,
    IValidator<UpdateChecklistItemRequestDto> updateItemValidator,
    IValidator<ReorderMasterChecklistItemsRequestDto> reorderItemsValidator) : ControllerBase
{

    [HttpGet(MasterChecklistConstants.Routes.CurrentDraft)]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> GetCurrentDraft()
    {
        var draftVersion = await checklistService.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return NotFound(new { message = MasterChecklistConstants.Messages.NoDraftVersion });
        }

        return Ok(draftVersion);
    }

    [HttpGet(MasterChecklistConstants.Routes.Published)]
    [Authorize(Roles = MasterChecklistConstants.Roles.ViewerRoles)]
    public async Task<IActionResult> GetPublished()
    {
        var publishedVersion = await checklistService.GetPublishedVersionAsync();
        if (publishedVersion is null)
        {
            return NotFound(new { message = MasterChecklistConstants.Messages.NoPublishedVersion });
        }

        return Ok(publishedVersion);
    }

    [HttpGet(MasterChecklistConstants.Routes.Versions)]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> GetVersionHistory()
    {
        var versions = await checklistService.GetVersionHistoryAsync();
        return Ok(versions);
    }

    [HttpGet(MasterChecklistConstants.Routes.Versions + "/{versionId}")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> GetVersionById(int versionId)
    {
        var result = await checklistService.GetVersionByIdAsync(versionId);
        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpGet(MasterChecklistConstants.Routes.Categories)]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await checklistService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost(MasterChecklistConstants.Routes.Items)]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> CreateItem([FromBody] MasterCreateChecklistItemRequestDto request)
    {
        var validationResult = await createItemValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid checklist item request.";
            return BadRequest(new { message = firstError });
        }

        var result = await checklistService.CreateChecklistItemAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPut(MasterChecklistConstants.Routes.Items + "/{id}")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateChecklistItemRequestDto request)
    {
        var validationResult = await updateItemValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid checklist item request.";
            return BadRequest(new { message = firstError });
        }

        var result = await checklistService.UpdateChecklistItemAsync(id, request);
        if (!result.IsSuccess)
        {
            return result.IsNotFound
                ? NotFound(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPut(MasterChecklistConstants.Routes.Items + "/reorder")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
    public async Task<IActionResult> ReorderItems([FromBody] ReorderMasterChecklistItemsRequestDto request)
    {
        var validationResult = await reorderItemsValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid reorder request.";
            return BadRequest(new { message = firstError });
        }

        var result = await checklistService.ReorderChecklistItemsAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPut(MasterChecklistConstants.Routes.Items + "/{id}/retire")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
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

    [HttpPost(MasterChecklistConstants.Routes.Versions + "/{versionId}/publish")]
    [Authorize(Policy = MasterChecklistConstants.Policies.AdminOnly)]
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
