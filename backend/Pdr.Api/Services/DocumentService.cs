using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pdr.Api.Services;

public class DocumentService(
    IDocumentRepository repository,
    IEngagementRepository engagementRepository,
    IWebHostEnvironment environment,
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    INotificationService notificationService,
    IValidator<DocumentUploadDto> uploadValidator,
    IAuditLogService auditLogService) : IDocumentService
{
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

    private async Task NotifyDocumentUploadedAsync(Document doc)
    {
        var currentUserId = currentUserService.GetUserId();

        var assignments = await engagementRepository.GetAssignmentsAsync(doc.EngagementId);
        if (assignments == null || !assignments.Any()) return;

        var payload = JsonSerializer.Serialize(
            new DocumentUploadedMapper.DocumentUploadedPayload(doc.Id, doc.EngagementId, currentUserId));

        foreach (var assignment in assignments)
        {
            if (assignment.UserId == currentUserId) continue;

            bool canAccess = false;
            if (doc.Visibility == DocumentVisibilityNames.TeamWide)
            {
                canAccess = true;
            }
            else if (doc.Visibility == DocumentVisibilityNames.PmOnly)
            {
                canAccess = assignment.User.RoleId == (int)RoleEnum.ProjectManagement || assignment.User.RoleId == (int)RoleEnum.Leadership;
            }
            else if (doc.Visibility == DocumentVisibilityNames.Custom)
            {
                canAccess = assignment.User.RoleId == (int)RoleEnum.ProjectManagement
                    || assignment.User.RoleId == (int)RoleEnum.Leadership
                    || (doc.AllowedRoles != null && doc.AllowedRoles.Contains(assignment.User.RoleId));
            }

            if (canAccess)
            {
                await notificationService.CreateFromTemplateAsync(
                    NotificationConstants.TemplateKeys.DocumentUploaded,
                    assignment.UserId,
                    payload,
                    DateTime.UtcNow);
            }
        }
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
        var engagementFolder = Path.Combine(environment.ContentRootPath, DocumentValidationConstants.UploadsFolder, dto.EngagementId.ToString());
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

        var contentType = string.IsNullOrWhiteSpace(dto.File.ContentType)
            ? DocumentValidationConstants.DefaultContentType
            : dto.File.ContentType;

        var fileSizeKb = (int)Math.Ceiling(dto.File.Length / 1024.0);

        var document = new Document
        {
            EngagementId = dto.EngagementId,
            CategoryId = dto.CategoryId,
            FileName = dto.File.FileName,
            FilePath = filePath,
            FileSizeKB = fileSizeKb,
            ContentType = contentType,
            Visibility = dto.Visibility
        };

        document.Versions.Add(new DocumentVersion
        {
            DocumentId = document.Id,
            VersionNumber = 1,
            FilePath = filePath,
            FileSizeKB = fileSizeKb,
            ContentType = contentType,
            IsCurrent = true
        });

        var saved = await repository.AddAsync(document);
        var reloaded = await repository.GetByIdAsync(saved.Id) ?? saved;

        await NotifyDocumentUploadedAsync(reloaded);

        // await NotifyDocumentUploadedAsync(reloaded);

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.DocumentUploaded,
            dto.EngagementId,
            new Dictionary<string, object?>
            {
                ["DocumentId"] = reloaded.Id,
                ["CategoryId"] = reloaded.CategoryId,
                ["ActorUserId"] = actorUserId,
                ["DocumentName"] = reloaded.FileName,
            },
            actingUserId: actorUserId);

        var uploaderNames = await GetUploaderNamesAsync([reloaded.CreatedBy]);
        return MapToResponseDto(reloaded, uploaderNames);
    }

    /// <summary>
    /// Uploads multiple documents, reporting a per-file result rather than failing the whole batch.
    /// A `List&lt;DocumentUploadDto&gt;` argument doesn't match any registered `IValidator&lt;List&lt;T&gt;&gt;`, so the
    /// global ValidationFilter can't validate per-item — each file is validated here instead.
    /// </summary>
    public async Task<List<DocumentUploadResultDto>> UploadDocumentsAsync(List<DocumentUploadDto> dtos)
    {
        if (dtos == null || dtos.Count == 0)
        {
            throw new NoDocumentsProvidedException();
        }

        var results = new List<DocumentUploadResultDto>();

        foreach (var dto in dtos)
        {
            var validationResult = await uploadValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                results.Add(new DocumentUploadResultDto
                {
                    FileName = dto.File?.FileName,
                    Success = false,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
                continue;
            }

            try
            {
                var uploaded = await UploadDocumentAsync(dto);
                results.Add(new DocumentUploadResultDto
                {
                    FileName = dto.File?.FileName,
                    Success = true,
                    Document = uploaded
                });
            }
            catch (AppException ex)
            {
                results.Add(new DocumentUploadResultDto
                {
                    FileName = dto.File?.FileName,
                    Success = false,
                    Errors = [ex.Message]
                });
            }
        }

        return results;
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsByEngagementAsync(int engagementId)
    {
        var userRoleId = currentUserService.GetRoleId();
        var isAdmin = currentUserService.IsAdmin();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);
        var docs = (await repository.GetByEngagementIdAsync(engagementId, userRoleId, isAdmin, isPm)).ToList();

        var uploaderNames = await GetUploaderNamesAsync(docs.Select(d => d.CreatedBy));
        return docs.Select(d => MapToResponseDto(d, uploaderNames));
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadDocumentAsync(int id)
    {
        var doc = await repository.GetByIdAsync(id);
        if (doc == null)
        {
            throw new DocumentNotFoundException(id);
        }

        if (!File.Exists(doc.FilePath))
        {
            throw new DocumentFileMissingException();
        }

        if (!CanAccessDocument(doc))
        {
            throw new DocumentAccessDeniedException(DocumentValidationConstants.Messages.DownloadDenied);
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
            throw new DocumentAccessDeniedException(DocumentValidationConstants.Messages.RemoveDenied);
        }

        // Signed SOW Guard
        bool isSignedSow = doc.CategoryId == (int)DocumentCategoryEnum.SignedSow
            || string.Equals(doc.Category?.CategoryName, DocumentCategoryNames.SignedSow, StringComparison.OrdinalIgnoreCase);
        if (isSignedSow && doc.Engagement?.HandoverInfo != null && doc.Engagement.HandoverInfo.StatusId == (int)HandoverStatusEnum.Submitted)
        {
            throw new HandoverSubmittedException(DocumentValidationConstants.Messages.SignedSowRemoveDenied);
        }

        var updatedBy = userEmail ?? userIdStr;
        return await repository.SoftDeleteAsync(doc, reason, updatedBy);
    }

    public async Task<DocumentResponseDto> UpdateDocumentVisibilityAsync(int id, UpdateDocumentVisibilityDto dto)
    {
        if (!CanManageDocumentVisibility())
        {
            throw new DocumentAccessDeniedException(DocumentValidationConstants.Messages.VisibilityChangeDenied);
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

        await NotifyDocumentUploadedAsync(updated);

        var uploaderNames = await GetUploaderNamesAsync([updated.CreatedBy]);
        return MapToResponseDto(updated, uploaderNames);
    }

    public async Task<DocumentResponseDto> UploadNewVersionAsync(int documentId, UploadDocumentVersionDto dto)
    {
        var doc = await repository.GetByIdAsync(documentId);
        if (doc == null)
        {
            throw new DocumentNotFoundException(documentId);
        }

        if (!CanAccessDocument(doc))
        {
            throw new DocumentAccessDeniedException(DocumentValidationConstants.Messages.UploadVersionDenied);
        }

        var engagementFolder = Path.Combine(environment.ContentRootPath, DocumentValidationConstants.UploadsFolder, doc.EngagementId.ToString());
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

        var nextVersionNumber = doc.Versions.Count == 0 ? 1 : doc.Versions.Max(v => v.VersionNumber) + 1;

        var newVersion = new DocumentVersion
        {
            DocumentId = doc.Id,
            VersionNumber = nextVersionNumber,
            FilePath = filePath,
            FileSizeKB = (int)Math.Ceiling(dto.File.Length / 1024.0),
            ContentType = string.IsNullOrWhiteSpace(dto.File.ContentType)
                ? DocumentValidationConstants.DefaultContentType
                : dto.File.ContentType,
            ChangeNote = dto.ChangeNote,
            IsCurrent = true
        };

        var updated = await repository.AddVersionAsync(doc, newVersion);
        var reloaded = await repository.GetByIdAsync(updated.Id) ?? updated;

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.DocumentVersioned,
            reloaded.EngagementId,
            new Dictionary<string, object?>
            {
                ["DocumentId"] = reloaded.Id,
                ["Version"] = newVersion.VersionNumber,
                ["ActorUserId"] = actorUserId,
                ["DocumentName"] = reloaded.FileName
            },
            actingUserId: actorUserId);

        var uploaderNames = await GetUploaderNamesAsync([reloaded.CreatedBy]);
        return MapToResponseDto(reloaded, uploaderNames);
    }

    public async Task<IEnumerable<DocumentVersionResponseDto>> GetVersionHistoryAsync(int documentId)
    {
        var doc = await repository.GetByIdAsync(documentId);
        if (doc == null)
        {
            throw new DocumentNotFoundException(documentId);
        }

        if (!CanAccessDocument(doc))
        {
            throw new DocumentAccessDeniedException(DocumentValidationConstants.Messages.DownloadDenied);
        }

        var versions = (await repository.GetVersionsByDocumentIdAsync(documentId)).ToList();
        var uploaderNames = await GetUploaderNamesAsync(versions.Select(v => v.CreatedBy));

        return versions.Select(v => new DocumentVersionResponseDto
        {
            Id = v.Id,
            VersionNumber = v.VersionNumber,
            IsCurrent = v.IsCurrent,
            UploadedBy = ResolveUploaderDisplayName(v.CreatedBy, uploaderNames),
            UploadedAt = v.CreatedAt,
            ChangeNote = v.ChangeNote
        });
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadVersionAsync(int documentId, int versionId)
    {
        var doc = await repository.GetByIdAsync(documentId);
        if (doc == null)
        {
            throw new DocumentNotFoundException(documentId);
        }

        if (!CanAccessDocument(doc))
        {
            throw new DocumentAccessDeniedException(DocumentValidationConstants.Messages.DownloadDenied);
        }

        var version = await repository.GetVersionByIdAsync(documentId, versionId);
        if (version == null || !File.Exists(version.FilePath))
        {
            throw new DocumentVersionNotFoundException();
        }

        var stream = new FileStream(version.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return (stream, version.ContentType, doc.FileName);
    }

    // ── Mapping ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Batch-resolves uploader IDs (stored as strings in the generic CreatedBy audit column)
    /// to full names in a single query, avoiding one user lookup per document.
    /// </summary>
    private async Task<Dictionary<int, string>> GetUploaderNamesAsync(IEnumerable<string?> createdByValues)
    {
        var userIds = createdByValues
            .Where(cb => int.TryParse(cb, out _))
            .Select(cb => int.Parse(cb!))
            .Distinct()
            .ToList();

        if (userIds.Count == 0)
        {
            return new Dictionary<int, string>();
        }

        var users = await userRepository.GetByIdsAsync(userIds);
        return users.ToDictionary(u => u.Id, u => u.FullName);
    }

    private static string ResolveUploaderDisplayName(string? createdBy, IReadOnlyDictionary<int, string> uploaderNames)
    {
        if (createdBy != null && int.TryParse(createdBy, out var id) && uploaderNames.TryGetValue(id, out var fullName))
        {
            return fullName;
        }

        return createdBy ?? string.Empty;
    }

    private static DocumentResponseDto MapToResponseDto(Document doc, IReadOnlyDictionary<int, string> uploaderNames)
    {
        var categoryDefault = GetCategoryDefaultVisibility(doc.Category?.CategoryName);
        return new DocumentResponseDto
        {
            Id = doc.Id,
            EngagementId = doc.EngagementId,
            CategoryId = doc.CategoryId,
            CategoryName = doc.Category?.CategoryName ?? string.Empty,
            FileName = doc.FileName,
            FileSizeKB = doc.FileSizeKB,
            ContentType = doc.ContentType,
            UploadedBy = ResolveUploaderDisplayName(doc.CreatedBy, uploaderNames),
            UploadedAt = doc.CreatedAt,
            Visibility = doc.Visibility,
            AllowedRoles = doc.AllowedRoles ?? new List<int>(),
            CategoryDefaultVisibility = categoryDefault,
            CurrentVersionNumber = doc.Versions.Count == 0 ? 1 : doc.Versions.Max(v => v.VersionNumber)
        };
    }
}
