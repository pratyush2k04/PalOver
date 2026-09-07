using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyCreatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyCreated);

    private record DependencyCreatedData(int DependencyId, string Type);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DependencyCreatedData>(data);
        if (payload is null) return;

        context.DependencyIds.Add(payload.DependencyId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DependencyCreatedData>(data)
            ?? throw new InvalidOperationException("DependencyCreated audit payload could not be parsed.");

        var dependency = context.Dependencies.GetValueOrDefault(payload.DependencyId);
        var engagement = dependency != null
            ? context.Engagements.GetValueOrDefault(dependency.EngagementId)
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
