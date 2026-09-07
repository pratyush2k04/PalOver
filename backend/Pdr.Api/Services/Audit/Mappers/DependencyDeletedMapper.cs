using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyDeletedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyDeleted);

    private record DependencyDeletedData(int DependencyId, int EngagementId, string? Description);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DependencyDeletedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.DependencyIds.Add(payload.DependencyId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DependencyDeletedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["dependencyDescription"] = TruncateDescription(payload.Description),
            ["engagementName"] = engagement?.EngagementName
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "a dependency";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
