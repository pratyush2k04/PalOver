using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentUploadedMapper(
    IDocumentRepository documentRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DocumentUploaded.TemplateKey;

    private record DocumentUploadedData(int DocumentId, int CategoryId, int ActorUserId, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DocumentUploaded.ParseErrorMessage);

        var doc = await documentRepository.GetByIdAsync(payload.DocumentId);
        var categories = await documentRepository.GetCategoriesAsync();
        var category = categories.FirstOrDefault(c => c.Id == payload.CategoryId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DocumentUploaded.DocumentNameKey] = doc?.FileName ?? payload.DocumentName ?? "Unknown Document",
            [AuditConstants.DocumentUploaded.CategoryNameKey] = category?.CategoryName ?? "Unknown Category",
            [AuditConstants.DocumentUploaded.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
