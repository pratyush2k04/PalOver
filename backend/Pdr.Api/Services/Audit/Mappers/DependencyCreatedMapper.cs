using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyCreatedMapper(
    IDependencyRepository dependencyRepository,
    IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyCreated);

    private record DependencyCreatedData(int DependencyId, string Type);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyCreatedData>(data)
            ?? throw new InvalidOperationException("DependencyCreated audit payload could not be parsed.");

        var dependency = await dependencyRepository.GetByIdAsync(payload.DependencyId);
        var engagement = dependency != null
            ? await engagementRepository.GetByIdAsync(dependency.EngagementId)
            : null;

        return new Dictionary<string, object?>
        {
            ["dependencyDescription"] = TruncateDescription(dependency?.Description),
            ["dependencyType"] = payload.Type,
            ["engagementName"] = engagement?.EngagementName ?? "Engagement"
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
