using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyUpdatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyUpdated);

    private record DependencyUpdatedData(int DependencyId, string Status);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DependencyUpdatedData>(data);
        if (payload is null) return;

        context.DependencyIds.Add(payload.DependencyId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DependencyUpdatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var dependency = context.Dependencies.GetValueOrDefault(payload.DependencyId);
        var engagement = dependency != null
            ? context.Engagements.GetValueOrDefault(dependency.EngagementId)
            : null;

        return new Dictionary<string, object?>
        {
            ["dependencyDescription"] = TruncateDescription(dependency?.Description),
            ["dependencyStatus"] = payload.Status,
            ["engagementName"] = engagement?.EngagementName
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
