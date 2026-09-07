using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyDeletedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyDeleted);

    private record DependencyDeletedData(int DependencyId, int EngagementId, string? Description);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyDeletedData>(data)
            ?? throw new InvalidOperationException("DependencyDeleted audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["dependencyDescription"] = TruncateDescription(payload.Description),
            ["engagementName"] = engagement?.EngagementName ?? "Engagement"
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
