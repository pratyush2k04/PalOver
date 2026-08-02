using Pdr.Api.Models.Documents;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);

    Task<Document?> GetByIdAsync(int id);

    Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId);

    Task<IEnumerable<DocumentCategory>> GetCategoriesAsync();

    Task<bool> SoftDeleteAsync(long id);
}
