using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Users;

namespace Pdr.Api.Services.Interfaces;

public interface IUserManagementService
{
    Task<PagedResult<UserListItemDto>> GetUsersAsync(string? search, int page, int pageSize);
    Task<UserListItemDto> ChangeUserRoleAsync(int targetUserId, int newRoleId);
}

public class UserNotFoundException(int userId) : Exception($"User with id {userId} was not found.");

public class RoleNotFoundException(int roleId) : Exception($"Role with id {roleId} was not found.");
