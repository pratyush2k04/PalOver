using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UnauthorizedAccessMapper : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.UnauthorizedAccess.TemplateKey;

    private record UnauthorizedAccessData(string? Email, int? UserId);

    public Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UnauthorizedAccessData>(data)
            ?? throw new InvalidOperationException(AuditConstants.UnauthorizedAccess.ParseErrorMessage);

        IDictionary<string, object?> result = new Dictionary<string, object?>
        {
            [AuditConstants.UnauthorizedAccess.EmailKey] = payload.Email ?? "unknown"
        };

        return Task.FromResult(result);
    }
}
