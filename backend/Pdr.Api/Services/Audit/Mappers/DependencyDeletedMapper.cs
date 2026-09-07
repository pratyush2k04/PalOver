using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyDeletedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DependencyDeleted.TemplateKey;

    private record DependencyDeletedData(int DependencyId, int EngagementId, string? Description);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyDeletedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DependencyDeleted.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DependencyDeleted.DependencyDescriptionKey] = TruncateDescription(payload.Description),
            [AuditConstants.DependencyDeleted.EngagementNameKey] = engagement?.EngagementName ?? "Engagement"
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
