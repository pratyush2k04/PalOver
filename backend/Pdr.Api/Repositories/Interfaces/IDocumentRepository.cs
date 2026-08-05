using Pdr.Api.Models.Documents;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);

    Task<Document?> GetByIdAsync(int id);

    Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId, bool includePmOnly);

    Task<IEnumerable<DocumentCategory>> GetCategoriesAsync();

    Task<bool> SoftDeleteAsync(int id, string reason, string updatedBy);

    Task<Document?> UpdateVisibilityAsync(int id, string visibility, string updatedBy);
}
