using Microsoft.AspNetCore.Hosting;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.Handover;
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

    // ── Helper methods ──────────────────────────────────────────────────────

    /// <summary>Only PM and Admin can change a document's visibility.</summary>
    private bool CanManageDocumentVisibility() =>
        currentUserService.IsInRole(RoleEnum.ProjectManagement)
        || currentUserService.IsAdmin();

    private bool CanAccessDocument(Document doc)
    {
        if (currentUserService.IsAdmin() || currentUserService.IsInRole(RoleEnum.ProjectManagement))
        {
            return true;
        }

        if (doc.Visibility == DocumentVisibilityNames.TeamWide)
        {
            return true;
        }

        if (doc.Visibility == DocumentVisibilityNames.PmOnly && currentUserService.IsInRole(RoleEnum.Leadership))
        {
            return true;
        }

        var roleId = currentUserService.GetRoleId();
        if (doc.Visibility == DocumentVisibilityNames.Custom && roleId.HasValue && doc.AllowedRoles != null && doc.AllowedRoles.Contains(roleId.Value))
        {
            return true;
        }

        return false;
    }

    private static string GetCategoryDefaultVisibility(string? categoryName)
    {
        return string.Equals(categoryName, DocumentCategoryNames.Commercial, StringComparison.OrdinalIgnoreCase)
            || string.Equals(categoryName, DocumentCategoryNames.Proposal, StringComparison.OrdinalIgnoreCase)
            ? DocumentVisibilityNames.PmOnly
            : DocumentVisibilityNames.TeamWide;
    }

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
        var userRoleId = currentUserService.GetRoleId();
        var isAdmin = currentUserService.IsAdmin();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);
        var docs = await repository.GetByEngagementIdAsync(engagementId, userRoleId, isAdmin, isPm);
        return docs.Select(MapToResponseDto);
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)?> DownloadDocumentAsync(int id)
    {
        var doc = await repository.GetByIdAsync(id);
        if (doc == null || !File.Exists(doc.FilePath))
        {
            return null;
        }

        if (!CanAccessDocument(doc))
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
            throw new DocumentNotFoundException(id);
        }

        var userIdStr = currentUserService.GetUserId().ToString();
        var userEmail = currentUserService.GetEmail();

        bool isUploader = string.Equals(doc.CreatedBy, userIdStr, StringComparison.OrdinalIgnoreCase)
            || (userEmail != null && string.Equals(doc.CreatedBy, userEmail, StringComparison.OrdinalIgnoreCase));

        bool isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);
        bool isAdmin = currentUserService.IsAdmin();

        if (!isUploader && !isPm && !isAdmin)
        {
            throw new DocumentAccessDeniedException("You do not have permission to remove this document. Only the uploader or a Project Manager can remove it.");
        }

        // Signed SOW Guard
        bool isSignedSow = doc.CategoryId == (int)DocumentCategoryEnum.SignedSow
            || string.Equals(doc.Category?.CategoryName, DocumentCategoryNames.SignedSow, StringComparison.OrdinalIgnoreCase);
        if (isSignedSow && doc.Engagement?.HandoverInfo != null && doc.Engagement.HandoverInfo.StatusId == (int)HandoverStatusEnum.Submitted)
        {
            throw new HandoverSubmittedException("The signed SOW cannot be removed because a submitted handover is relying on it.");
        }

        var updatedBy = userEmail ?? userIdStr;
        return await repository.SoftDeleteAsync(doc, reason, updatedBy);
    }

    public async Task<DocumentResponseDto> UpdateDocumentVisibilityAsync(int id, UpdateDocumentVisibilityDto dto)
    {
        if (!CanManageDocumentVisibility())
        {
            throw new DocumentAccessDeniedException("You do not have permission to change document visibility. Only Project Managers and Admins can do this.");
        }

        var doc = await repository.GetByIdAsync(id);
        if (doc == null)
        {
            throw new DocumentNotFoundException(id);
        }

        var targetVisibility = dto.ResetToCategoryDefault
            ? GetCategoryDefaultVisibility(doc.Category?.CategoryName)
            : dto.Visibility ?? GetCategoryDefaultVisibility(doc.Category?.CategoryName);

        var allowedRoles = dto.AllowedRoles ?? new List<int>();

        if (dto.ResetToCategoryDefault || targetVisibility != DocumentVisibilityNames.Custom)
        {
            allowedRoles = new List<int>();
        }

        var userEmail = currentUserService.GetEmail();
        var updatedBy = userEmail ?? currentUserService.GetUserId().ToString();

        var updated = await repository.UpdateVisibilityAsync(doc, targetVisibility, allowedRoles, updatedBy);

        return MapToResponseDto(updated);
    }

    // ── Mapping ─────────────────────────────────────────────────────────────

    private static DocumentResponseDto MapToResponseDto(Document doc)
    {
        var categoryDefault = GetCategoryDefaultVisibility(doc.Category?.CategoryName);
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
            Visibility = doc.Visibility,
            AllowedRoles = doc.AllowedRoles ?? new List<int>(),
            CategoryDefaultVisibility = categoryDefault
        };
    }
}
