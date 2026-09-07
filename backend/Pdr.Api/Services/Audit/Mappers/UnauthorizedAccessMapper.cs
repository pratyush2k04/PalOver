using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UnauthorizedAccessMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UnauthorizedAccess);

    private record UnauthorizedAccessData(string? Email, int? UserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<UnauthorizedAccessData>(data);
        if (payload is null) return;


    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<UnauthorizedAccessData>(data)
            ?? throw new InvalidOperationException("UnauthorizedAccess audit payload could not be parsed.");

        return new Dictionary<string, object?>
        {
            ["email"] = payload.Email ?? "unknown"
        };

        
    }
}
