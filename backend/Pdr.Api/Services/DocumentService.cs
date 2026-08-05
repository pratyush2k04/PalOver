using Microsoft.AspNetCore.Hosting;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Documents;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdr.Api.Services;

public class DocumentService(
    IDocumentRepository repository,
    IWebHostEnvironment environment,
    ICurrentUserService currentUserService) : IDocumentService
{
    private const string UploadsFolder = "Uploads/Documents";

    // ── Role-check helpers ──────────────────────────────────────────────────

    /// <summary>PM, Leadership, and Admin users can see and download PM-only documents.</summary>
    private bool CanAccessPmOnlyDocuments() =>
        currentUserService.IsInRole("Project Management")
        || currentUserService.IsInRole("Leadership")
        || currentUserService.IsAdmin();

    /// <summary>Only PM and Admin can change a document's visibility.</summary>
    private bool CanManageDocumentVisibility() =>
        currentUserService.IsInRole("Project Management")
        || currentUserService.IsAdmin();

    // ── CRUD ────────────────────────────────────────────────────────────────

    public async Task<DocumentResponseDto> UploadDocumentAsync(DocumentUploadDto dto)
    {
        var engagementFolder = Path.Combine(environment.ContentRootPath, UploadsFolder, dto.EngagementId.ToString());
        if (!Directory.Exists(engagementFolder))
        {
            Directory.CreateDirectory(engagementFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
        var filePath = Path.Combine(engagementFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var document = new Document
        {
            EngagementId = dto.EngagementId,
            CategoryId = dto.CategoryId,
            FileName = dto.File.FileName,
            FilePath = filePath,
            FileSizeBytes = dto.File.Length,
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType)
                ? "application/octet-stream"
                : dto.File.ContentType,
            Visibility = dto.Visibility
        };

        var saved = await repository.AddAsync(document);
        var reloaded = await repository.GetByIdAsync(saved.Id) ?? saved;

        return MapToResponseDto(reloaded);
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsByEngagementAsync(int engagementId)
    {
        var includePmOnly = CanAccessPmOnlyDocuments();
        var docs = await repository.GetByEngagementIdAsync(engagementId, includePmOnly);
        return docs.Select(MapToResponseDto);
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)?> DownloadDocumentAsync(int id)
    {
        var doc = await repository.GetByIdAsync(id);
        if (doc == null || !File.Exists(doc.FilePath))
        {
            return null;
        }

        // Enforce PM-only visibility on download
        if (doc.Visibility == "PM-only" && !CanAccessPmOnlyDocuments())
        {
            throw new DocumentAccessDeniedException("You do not have permission to download this document.");
        }

        var stream = new FileStream(doc.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return (stream, doc.ContentType, doc.FileName);
    }

    public async Task<IEnumerable<DocumentCategoryDto>> GetCategoriesAsync()
    {
        var categories = await repository.GetCategoriesAsync();
        return categories.Select(c => new DocumentCategoryDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName
        });
    }

    public async Task<bool> DeleteDocumentAsync(int id, string reason)
    {
        var doc = await repository.GetByIdAsync(id);
        if (doc == null || doc.IsDeleted)
        {
            return false;
        }

        var userIdStr = currentUserService.GetUserId().ToString();
        var userEmail = currentUserService.GetEmail();

        bool isUploader = string.Equals(doc.CreatedBy, userIdStr, StringComparison.OrdinalIgnoreCase)
            || (userEmail != null && string.Equals(doc.CreatedBy, userEmail, StringComparison.OrdinalIgnoreCase));

        bool isPm = currentUserService.IsInRole("Project Management");
        bool isAdmin = currentUserService.IsAdmin();

        if (!isUploader && !isPm && !isAdmin)
        {
            throw new UnauthorizedAccessException("You do not have permission to remove this document. Only the uploader or a Project Manager can remove it.");
        }

        // Signed SOW Guard (CategoryId = 3 or CategoryName = "Signed SOW")
        bool isSignedSow = doc.CategoryId == 3 || string.Equals(doc.Category?.CategoryName, "Signed SOW", StringComparison.OrdinalIgnoreCase);
        if (isSignedSow && doc.Engagement?.HandoverInfo != null && doc.Engagement.HandoverInfo.StatusId == 2)
        {
            throw new InvalidOperationException("The signed SOW cannot be removed because a submitted handover is relying on it.");
        }

        var updatedBy = userEmail ?? userIdStr;
        return await repository.SoftDeleteAsync(id, reason, updatedBy);
    }

    public async Task<DocumentResponseDto> UpdateDocumentVisibilityAsync(int id, string visibility)
    {
        if (!CanManageDocumentVisibility())
        {
            throw new DocumentAccessDeniedException("You do not have permission to change document visibility. Only Project Managers and Admins can do this.");
        }

        var userEmail = currentUserService.GetEmail();
        var updatedBy = userEmail ?? currentUserService.GetUserId().ToString();

        var updated = await repository.UpdateVisibilityAsync(id, visibility, updatedBy);
        if (updated == null)
        {
            throw new KeyNotFoundException($"Document with id {id} was not found.");
        }

        return MapToResponseDto(updated);
    }

    // ── Mapping ─────────────────────────────────────────────────────────────

    private static DocumentResponseDto MapToResponseDto(Document doc)
    {
        return new DocumentResponseDto
        {
            Id = doc.Id,
            EngagementId = doc.EngagementId,
            CategoryId = doc.CategoryId,
            CategoryName = doc.Category?.CategoryName ?? string.Empty,
            FileName = doc.FileName,
            FileSizeBytes = doc.FileSizeBytes,
            ContentType = doc.ContentType,
            UploadedBy = doc.CreatedBy ?? string.Empty,
            UploadedAt = doc.CreatedAt,
            Visibility = doc.Visibility
        };
    }
}
