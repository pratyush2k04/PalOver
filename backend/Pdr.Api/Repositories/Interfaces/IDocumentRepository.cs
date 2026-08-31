using Pdr.Api.Models.Documents;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);

    Task<Document?> GetByIdAsync(int id);

    Task<IEnumerable<Document>> GetByEngagementIdAsync(int engagementId, int? userRoleId, bool isAdmin, bool isPm);

    Task<IEnumerable<DocumentCategory>> GetCategoriesAsync();

    Task<bool> SoftDeleteAsync(Document document, string reason, string updatedBy);

    Task<Document> UpdateVisibilityAsync(Document document, string visibility, List<int> allowedRoles, string updatedBy);

    Task<IEnumerable<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId);

    Task<DocumentVersion?> GetVersionByIdAsync(int documentId, int versionId);

    Task<Document> AddVersionAsync(Document document, DocumentVersion newVersion);}
