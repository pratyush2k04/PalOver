using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Services.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string? GetRole();
    int? GetRoleId();
    UserRoleEnum? GetUserRole();
    string? GetEmail();
    string? GetFullName();
    bool IsInRole(string role);
    bool IsInRole(int roleId);
    bool IsInUserRole(UserRoleEnum role);
    bool IsAdmin();
}
