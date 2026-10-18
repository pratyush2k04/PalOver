using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class AccountLockoutMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.AccountLockout);

    private record AccountLockoutData(int UserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<AccountLockoutData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.UserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<AccountLockoutData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var user = context.Users.GetValueOrDefault(payload.UserId);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName ?? user?.Email
        };
    }
}
