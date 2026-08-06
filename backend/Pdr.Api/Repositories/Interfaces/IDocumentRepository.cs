using Pdr.Api.Models.Documents;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);

    Task<Document?> GetByIdAsync(int id);

    Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId, string? userRole, bool isAdmin, bool isPm);

    Task<IEnumerable<DocumentCategory>> GetCategoriesAsync();

    Task<bool> SoftDeleteAsync(int id, string reason, string updatedBy);

    Task<Document?> UpdateVisibilityAsync(int id, string visibility, List<string> allowedRoles, string updatedBy);
}
