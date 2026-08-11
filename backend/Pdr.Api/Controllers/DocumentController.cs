using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages engagement document uploads, downloads, visibility, and removals.
/// </summary>
[Authorize]
[ApiController]
[Route("api/documents")]
[Produces("application/json")]
public class DocumentController(
    IDocumentService documentService,
    IValidator<DocumentUploadDto> uploadValidator) : ControllerBase
{
    /// <summary>
    /// Uploads one or more documents for an engagement.
    /// </summary>
    /// <param name="dtos">The list of document upload payloads.</param>
    /// <returns>Upload processing results for each document.</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
    [EndpointSummary("Upload engagement documents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadDocuments([FromForm] List<DocumentUploadDto> dtos)
    {
        if (dtos == null || dtos.Count == 0)
        {
            return BadRequest(new { message = DocumentValidationConstants.Messages.NoDocumentsProvided });
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

    /// <summary>
    /// Retrieves documents for an engagement based on user authorization.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>A list of accessible engagement documents.</returns>
    [HttpGet("engagement/{engagementId:int}")]
    [EndpointSummary("Get documents by engagement ID")]
    [ProducesResponseType(typeof(List<DocumentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEngagementDocuments(int engagementId)
    {
        var docs = await documentService.GetDocumentsByEngagementAsync(engagementId);
        return Ok(docs);
    }

    /// <summary>
    /// Downloads a document by ID if authorized.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <returns>The requested document file stream.</returns>
    [HttpGet("{id:int}/download")]
    [EndpointSummary("Download a document file")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var result = await documentService.DownloadDocumentAsync(id);
        if (result == null)
        {
            return NotFound(new { message = DocumentValidationConstants.Messages.FileNotFound });
        }

        return File(result.Value.FileStream, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Retrieves document category lookup data.
    /// </summary>
    /// <returns>A list of document categories.</returns>
    [HttpGet("categories")]
    [AllowAnonymous]
    [EndpointSummary("Get document categories")]
    [ProducesResponseType(typeof(List<DocumentCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await documentService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Soft deletes a document with a mandatory reason.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <param name="request">The removal reason payload.</param>
    /// <returns>No content on successful deletion.</returns>
    [HttpPost("{id:int}/remove")]
    [EndpointSummary("Soft delete a document with reason")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(int id, [FromBody] RemoveDocumentRequestDto request)
    {
        await documentService.DeleteDocumentAsync(id, request.Reason);
        return NoContent();
    }

    /// <summary>
    /// Updates the access visibility of a document.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <param name="request">The updated visibility payload.</param>
    /// <returns>The updated document details.</returns>
    [HttpPatch("{id:int}/visibility")]
    [EndpointSummary("Update document access visibility")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVisibility(int id, [FromBody] UpdateDocumentVisibilityDto request)
    {
        var updated = await documentService.UpdateDocumentVisibilityAsync(id, request);
        return Ok(updated);
    }
}
