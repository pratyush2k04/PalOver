using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Documents;
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
            .Include(d => d.Engagement)
                .ThenInclude(e => e.HandoverInfo)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId, bool includePmOnly)
    {
        var query = context.Documents
            .Include(d => d.Category)
            .Where(d => d.EngagementId == engagementId);

        if (!includePmOnly)
        {
            query = query.Where(d => d.Visibility != "PM-only");
        }

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DocumentCategory>> GetCategoriesAsync()
    {
        return await context.DocumentCategories
            .OrderBy(c => c.CategoryName == "Other" ? 1 : 0)
            .ThenBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<bool> SoftDeleteAsync(int id, string reason, string updatedBy)
    {
        var doc = await context.Documents.FindAsync(id);
        if (doc == null || doc.IsDeleted) return false;

        doc.IsDeleted = true;
        doc.DeletionReason = reason;
        doc.UpdatedAt = DateTime.UtcNow;
        doc.UpdatedBy = updatedBy;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Document?> UpdateVisibilityAsync(int id, string visibility, string updatedBy)
    {
        var doc = await context.Documents
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doc == null) return null;

        doc.Visibility = visibility;
        doc.UpdatedAt = DateTime.UtcNow;
        doc.UpdatedBy = updatedBy;
        await context.SaveChangesAsync();
        return doc;
    }
}
