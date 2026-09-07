using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyUpdatedMapper(
    IDependencyRepository dependencyRepository,
    IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DependencyUpdated.TemplateKey;

    private record DependencyUpdatedData(int DependencyId, string Status);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyUpdatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DependencyUpdated.ParseErrorMessage);

        var dependency = await dependencyRepository.GetByIdAsync(payload.DependencyId);
        var engagement = dependency != null
            ? await engagementRepository.GetByIdAsync(dependency.EngagementId)
            : null;

        return new Dictionary<string, object?>
        {
            [AuditConstants.DependencyUpdated.DependencyDescriptionKey] = TruncateDescription(dependency?.Description),
            [AuditConstants.DependencyUpdated.DependencyStatusKey] = payload.Status,
            [AuditConstants.DependencyUpdated.EngagementNameKey] = engagement?.EngagementName ?? "Engagement"
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
