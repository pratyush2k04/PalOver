using Pdr.Api.Models.Documents;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);

    Task<Document?> GetByIdAsync(int id);

    Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId, int? userRoleId, bool isAdmin, bool isPm);

    Task<IEnumerable<DocumentCategory>> GetCategoriesAsync();

    Task<bool> SoftDeleteAsync(int id, string reason, string updatedBy);

    Task<Document?> UpdateVisibilityAsync(int id, string visibility, List<int> allowedRoles, string updatedBy);
}
