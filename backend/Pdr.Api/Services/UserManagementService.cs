using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Users;
using Pdr.Api.Repositories;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository) : IUserManagementService
{
    public async Task<PagedResult<UserListItemDto>> GetUsersAsync(string? search, int page, int pageSize)
    {
        var (users, totalCount) = await userRepository.SearchAsync(search, page, pageSize);

        return new PagedResult<UserListItemDto>
        {
            Items = users.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserListItemDto> ChangeUserRoleAsync(int targetUserId, int newRoleId)
    {
        var user = await userRepository.GetByIdAsync(targetUserId);

        if (user.RoleId != newRoleId)
        {
            var newRole = await roleRepository.GetByIdAsync(newRoleId);
                

            user.RoleId = newRole.Id;
            user.Role = newRole;
            await userRepository.UpdateAsync(user);
        }

        return ToDto(user);
    }

    private static UserListItemDto ToDto(Models.User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        RoleId = user.RoleId,
        RoleName = user.Role.Name
    };
}
