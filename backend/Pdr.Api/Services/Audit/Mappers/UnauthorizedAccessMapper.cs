using System.Text.Json;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UnauthorizedAccessMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UnauthorizedAccess);

    private record UnauthorizedAccessData(string? Email, int? UserId);

    public Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UnauthorizedAccessData>(data)
            ?? throw new InvalidOperationException("UnauthorizedAccess audit payload could not be parsed.");

        IDictionary<string, object?> result = new Dictionary<string, object?>
        {
            ["email"] = payload.Email ?? "unknown"
        };

        return Task.FromResult(result);
    }
}
