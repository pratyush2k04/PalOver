using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserRoleAllocatedMapper(
    IUserRepository userRepository,
    IRoleRepository roleRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.UserRoleAllocated.TemplateKey;

    private record UserRoleAllocatedData(int TargetUserId, int RoleId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserRoleAllocatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.UserRoleAllocated.ParseErrorMessage);

        var targetUser = await userRepository.GetByIdAsync(payload.TargetUserId);
        var role = await roleRepository.GetByIdAsync(payload.RoleId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.UserRoleAllocated.RoleNameKey] = role?.Name ?? "Role",
            [AuditConstants.UserRoleAllocated.TargetUserNameKey] = targetUser?.FullName ?? "User",
            [AuditConstants.UserRoleAllocated.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
