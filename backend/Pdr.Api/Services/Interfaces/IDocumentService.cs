using Pdr.Api.DTOs.Documents;
using System.IO;

namespace Pdr.Api.Services.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> UploadDocumentAsync(DocumentUploadDto dto);

    Task<IEnumerable<DocumentResponseDto>> GetDocumentsByEngagementAsync(int engagementId);

    Task<(Stream FileStream, string ContentType, string FileName)?> DownloadDocumentAsync(int id);

    Task<IEnumerable<DocumentCategoryDto>> GetCategoriesAsync();

    Task<bool> DeleteDocumentAsync(int id, string reason);
}
