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
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId)
    {
        return await context.Documents
            .Include(d => d.Category)
            .Where(d => d.EngagementId == engagementId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DocumentCategory>> GetCategoriesAsync()
    {
        return await context.DocumentCategories
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<bool> SoftDeleteAsync(long id)
    {
        var doc = await context.Documents.FindAsync(id);
        if (doc == null || doc.IsDeleted) return false;

        doc.IsDeleted = true;
        await context.SaveChangesAsync();
        return true;
    }
}
