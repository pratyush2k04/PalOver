using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Services.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string? GetRole();
    int? GetRoleId();
    RoleEnum? GetRoleEnum();
    string? GetEmail();
    string? GetFullName();
    bool IsInRole(string role);
    bool IsInRole(RoleEnum role);
    bool IsInRole(int roleId);
    bool IsAdmin();
}
