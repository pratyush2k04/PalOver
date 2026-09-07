using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserRoleAllocatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserRoleAllocated);

    private record UserRoleAllocatedData(int TargetUserId, int RoleId, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<UserRoleAllocatedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.UserIds.Add(payload.TargetUserId);
        context.RoleIds.Add(payload.RoleId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<UserRoleAllocatedData>(data)
            ?? throw new InvalidOperationException("UserRoleAllocated audit payload could not be parsed.");

        var targetUser = context.Users.GetValueOrDefault(payload.TargetUserId);
        var role = context.Roles.GetValueOrDefault(payload.RoleId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["roleName"] = role?.Name ?? "Role",
            ["targetUserName"] = targetUser?.FullName ?? "User",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
