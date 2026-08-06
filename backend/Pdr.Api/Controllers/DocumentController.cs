using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pdr.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/documents")]
public class DocumentController(
    IDocumentService documentService,
    IValidator<DocumentUploadDto> uploadValidator,
    IValidator<RemoveDocumentRequestDto> removeValidator,
    IValidator<UpdateDocumentVisibilityDto> visibilityValidator) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocuments([FromForm] List<DocumentUploadDto> dtos)
    {
        if (dtos == null || dtos.Count == 0)
        {
            return BadRequest(new { message = "No documents provided." });
        }

        var results = new List<object>();
        var hasErrors = false;

        foreach (var dto in dtos)
        {
            var validationResult = await uploadValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                hasErrors = true;
                results.Add(new
                {
                    fileName = dto.File?.FileName,
                    success = false,
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
                continue;
            }

            try
            {
                var uploaded = await documentService.UploadDocumentAsync(dto);
                results.Add(new
                {
                    fileName = dto.File?.FileName,
                    success = true,
                    document = uploaded
                });
            }
            catch (Exception ex)
            {
                hasErrors = true;
                results.Add(new
                {
                    fileName = dto.File?.FileName,
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
        }

        // Return 207 Multi-Status if there's a mix of success and failure, or 200/400
        if (hasErrors && results.Any(r => (bool)((dynamic)r).success))
        {
            return StatusCode(207, results);
        }
        else if (hasErrors)
        {
            return BadRequest(results);
        }

        return Ok(results);
    }

    [HttpGet("engagement/{engagementId:int}")]
    public async Task<IActionResult> GetEngagementDocuments(int engagementId)
    {
        var docs = await documentService.GetDocumentsByEngagementAsync(engagementId);
        return Ok(docs);
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var result = await documentService.DownloadDocumentAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Document not found or file missing." });
        }

        return File(result.Value.FileStream, result.Value.ContentType, result.Value.FileName);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await documentService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost("{id:int}/remove")]
    public async Task<IActionResult> DeleteDocument(int id, [FromBody] RemoveDocumentRequestDto request)
    {
        await removeValidator.ValidateAndThrowAsync(request);
        await documentService.DeleteDocumentAsync(id, request.Reason);
        return NoContent();
    }

    [HttpPatch("{id:int}/visibility")]
    public async Task<IActionResult> UpdateVisibility(int id, [FromBody] UpdateDocumentVisibilityDto request)
    {
        await visibilityValidator.ValidateAndThrowAsync(request);
        var updated = await documentService.UpdateDocumentVisibilityAsync(id, request);
        return Ok(updated);
    }
}
