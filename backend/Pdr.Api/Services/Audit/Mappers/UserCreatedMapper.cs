using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserCreatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserCreated);

    private record UserCreatedData(int UserId, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<UserCreatedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.UserIds.Add(payload.UserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<UserCreatedData>(data)
            ?? throw new InvalidOperationException("UserCreated audit payload could not be parsed.");

        var user = context.Users.GetValueOrDefault(payload.UserId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName,
            ["actorName"] = actor?.FullName
        };
    }
}
