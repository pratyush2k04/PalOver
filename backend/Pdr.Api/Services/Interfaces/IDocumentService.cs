using Pdr.Api.DTOs.Documents;
using System.IO;

namespace Pdr.Api.Services.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> UploadDocumentAsync(int engagementId, DocumentUploadDto dto);

    Task<List<DocumentUploadResultDto>> UploadDocumentsAsync(int engagementId, List<DocumentUploadDto> dtos);

    Task<IEnumerable<DocumentResponseDto>> GetDocumentsByEngagementAsync(int engagementId);

    Task<(Stream FileStream, string ContentType, string FileName)> DownloadDocumentAsync(int id);

    Task<IEnumerable<DocumentCategoryDto>> GetCategoriesAsync();

    Task<bool> DeleteDocumentAsync(int id, string reason);

    Task<DocumentResponseDto> UpdateDocumentVisibilityAsync(int id, UpdateDocumentVisibilityDto dto);

    Task<DocumentResponseDto> UploadNewVersionAsync(int documentId, UploadDocumentVersionDto dto);

    Task<IEnumerable<DocumentVersionResponseDto>> GetVersionHistoryAsync(int documentId);

    Task<(Stream FileStream, string ContentType, string FileName)> DownloadVersionAsync(int documentId, int versionId);
}
