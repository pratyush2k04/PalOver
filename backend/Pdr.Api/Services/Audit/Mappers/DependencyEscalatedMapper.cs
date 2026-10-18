using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyEscalatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyEscalated);

    private record DependencyEscalatedData(int DependencyId, int RiskId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DependencyEscalatedData>(data);
        if (payload is null) return;

        context.DependencyIds.Add(payload.DependencyId);
        context.RiskIds.Add(payload.RiskId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DependencyEscalatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var dependency = context.Dependencies.GetValueOrDefault(payload.DependencyId);
        var engagement = dependency != null
            ? context.Engagements.GetValueOrDefault(dependency.EngagementId)
            : null;
        var risk = context.Risks.GetValueOrDefault(payload.RiskId);

        return new Dictionary<string, object?>
        {
            ["dependencyDescription"] = TruncateDescription(dependency?.Description),
            ["riskTitle"] = risk?.Title,
            ["engagementName"] = engagement?.EngagementName
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
