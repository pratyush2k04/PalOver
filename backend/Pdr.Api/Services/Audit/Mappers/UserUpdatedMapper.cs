using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserUpdatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserUpdated);

    private record UserUpdatedData(int UserId, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<UserUpdatedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.UserIds.Add(payload.UserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<UserUpdatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var user = context.Users.GetValueOrDefault(payload.UserId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName,
            ["actorName"] = actor?.FullName
        };
    }
}
