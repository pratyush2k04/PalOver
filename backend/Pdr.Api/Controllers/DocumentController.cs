using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
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
public class DocumentController(IDocumentService documentService) : ControllerBase
{
    /// <summary>
    /// Uploads one or more documents for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="dtos">The list of document upload payloads.</param>
    /// <returns>Upload processing results for each document.</returns>
    [HttpPost("{engagementId:int}/upload")]
    [RequireEngagementAccess]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
    [EndpointSummary("Upload engagement documents")]
    [ProducesResponseType(typeof(List<DocumentUploadResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<DocumentUploadResultDto>), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(typeof(List<DocumentUploadResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadDocuments(int engagementId, [FromForm] List<DocumentUploadDto> dtos)
    {
        var results = await documentService.UploadDocumentsAsync(engagementId, dtos);

        var hasErrors = results.Any(r => !r.Success);
        if (hasErrors && results.Any(r => r.Success))
        {
            return StatusCode(207, results);
        }

        return hasErrors ? BadRequest(results) : Ok(results);
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
        return File(result.FileStream, result.ContentType, result.FileName);
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

    /// <summary>
    /// Uploads a new version of an existing document, superseding the current one.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <param name="dto">The new file and optional change note.</param>
    /// <returns>The updated document with its new current version.</returns>
    [HttpPost("{id:int}/versions")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
    [EndpointSummary("Upload a new document version")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadNewVersion(int id, [FromForm] UploadDocumentVersionDto dto)
    {
        var updated = await documentService.UploadNewVersionAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Retrieves the version history for a document, newest first.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <returns>All versions of the document.</returns>
    [HttpGet("{id:int}/versions")]
    [EndpointSummary("Get document version history")]
    [ProducesResponseType(typeof(List<DocumentVersionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersionHistory(int id)
    {
        var versions = await documentService.GetVersionHistoryAsync(id);
        return Ok(versions);
    }

    /// <summary>
    /// Downloads a specific historical version of a document.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <param name="versionId">The version ID to download.</param>
    /// <returns>The requested version's file stream.</returns>
    [HttpGet("{id:int}/versions/{versionId:int}/download")]
    [EndpointSummary("Download a specific document version")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadVersion(int id, int versionId)
    {
        var result = await documentService.DownloadVersionAsync(id, versionId);
        return File(result.FileStream, result.ContentType, result.FileName);
    }
}
