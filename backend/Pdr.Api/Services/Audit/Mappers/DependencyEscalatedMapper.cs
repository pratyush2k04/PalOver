using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyEscalatedMapper(
    IDependencyRepository dependencyRepository,
    IEngagementRepository engagementRepository,
    IRiskRepository riskRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DependencyEscalated.TemplateKey;

    private record DependencyEscalatedData(int DependencyId, int RiskId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyEscalatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DependencyEscalated.ParseErrorMessage);

        var dependency = await dependencyRepository.GetByIdAsync(payload.DependencyId);
        var engagement = dependency != null
            ? await engagementRepository.GetByIdAsync(dependency.EngagementId)
            : null;
        var risk = await riskRepository.GetByIdAsync(payload.RiskId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DependencyEscalated.DependencyDescriptionKey] = TruncateDescription(dependency?.Description),
            [AuditConstants.DependencyEscalated.RiskTitleKey] = risk?.Title ?? "Escalated Risk",
            [AuditConstants.DependencyEscalated.EngagementNameKey] = engagement?.EngagementName ?? "Engagement"
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
