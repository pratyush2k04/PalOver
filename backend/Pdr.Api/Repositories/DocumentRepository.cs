using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class DocumentRepository(AppDbContext context) : IDocumentRepository
{
    public async Task<Document> AddAsync(Document document)
    {
        await context.Documents.AddAsync(document);
        await context.SaveChangesAsync();
        return document;
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await context.Documents
            .Include(d => d.Category)
            .Include(d => d.Versions)
            .Include(d => d.Engagement)
                .ThenInclude(e => e.HandoverInfo)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId, int? userRoleId, bool isAdmin, bool isPm)
    {
        var query = context.Documents
            .Include(d => d.Category)
            .Include(d => d.Versions)
            .Where(d => d.EngagementId == engagementId);

        if (!isAdmin && !isPm)
        {
            query = query.Where(d =>
                d.Visibility == DocumentVisibilityNames.TeamWide
                || (d.Visibility == DocumentVisibilityNames.PmOnly && userRoleId == (int)RoleEnum.Leadership)
                || (d.Visibility == DocumentVisibilityNames.Custom && userRoleId.HasValue && d.AllowedRoles.Contains(userRoleId.Value)));
        }

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DocumentCategory>> GetCategoriesAsync()
    {
        return await context.DocumentCategories
            .OrderBy(c => c.CategoryName == DocumentCategoryNames.Other ? 1 : 0)
            .ThenBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<bool> SoftDeleteAsync(Document document, string reason, string updatedBy)
    {
        document.IsDeleted = true;
        document.DeletionReason = reason;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = updatedBy;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Document> UpdateVisibilityAsync(Document document, string visibility, List<int> allowedRoles, string updatedBy)
    {
        document.Visibility = visibility;
        document.AllowedRoles = allowedRoles ?? new List<int>();
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = updatedBy;
        await context.SaveChangesAsync();
        return document;
    }

    public async Task<IEnumerable<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId)
    {
        return await context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<DocumentVersion?> GetVersionByIdAsync(int documentId, int versionId)
    {
        return await context.DocumentVersions
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.Id == versionId);
    }

    public async Task<Document> AddVersionAsync(Document document, DocumentVersion newVersion)
    {
        var currentVersion = document.Versions.FirstOrDefault(v => v.IsCurrent);
        if (currentVersion != null)
        {
            currentVersion.IsCurrent = false;
        }

        document.Versions.Add(newVersion);
        document.FilePath = newVersion.FilePath;
        document.FileSizeKB = newVersion.FileSizeKB;
        document.ContentType = newVersion.ContentType;

        await context.SaveChangesAsync();
        return document;
    }}
