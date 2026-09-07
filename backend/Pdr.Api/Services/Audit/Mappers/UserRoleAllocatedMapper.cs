using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserRoleAllocatedMapper(
    IUserRepository userRepository,
    IRoleRepository roleRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserRoleAllocated);

    private record UserRoleAllocatedData(int TargetUserId, int RoleId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserRoleAllocatedData>(data)
            ?? throw new InvalidOperationException("UserRoleAllocated audit payload could not be parsed.");

        var targetUser = await userRepository.GetByIdAsync(payload.TargetUserId);
        var role = await roleRepository.GetByIdAsync(payload.RoleId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["roleName"] = role?.Name ?? "Role",
            ["targetUserName"] = targetUser?.FullName ?? "User",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
