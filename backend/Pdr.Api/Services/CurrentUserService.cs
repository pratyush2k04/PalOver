using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Pdr.Api.Constants;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public int GetUserId()
    {
        var subClaim = User?.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? User?.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException(AuthConstants.Messages.UnauthenticatedUser);

        return int.Parse(subClaim.Value);
    }

    public string? GetRole()
    {
        return User?.FindFirst(ClaimTypes.Role)?.Value
            ?? User?.FindFirst(ClaimConstants.Role)?.Value;
    }

    public int? GetRoleId()
    {
        var roleIdClaim = User?.FindFirst(ClaimConstants.RoleId)?.Value
            ?? User?.FindFirst(ClaimConstants.RoleIdSnake)?.Value;

        if (roleIdClaim != null && int.TryParse(roleIdClaim, out var roleId))
        {
            return roleId;
        }

        var roleName = GetRole();
        if (roleName != null)
        {
            if (string.Equals(roleName, UserRoleConstants.PreSales, StringComparison.OrdinalIgnoreCase))
                return (int)UserRoleEnum.PreSales;
            if (string.Equals(roleName, UserRoleConstants.Leadership, StringComparison.OrdinalIgnoreCase))
                return (int)UserRoleEnum.Leadership;
            if (string.Equals(roleName, UserRoleConstants.Delivery, StringComparison.OrdinalIgnoreCase))
                return (int)UserRoleEnum.Delivery;
            if (string.Equals(roleName, EngagementRoleConstants.ProjectManager, StringComparison.OrdinalIgnoreCase))
                return (int)RoleEnum.ProjectManagement;
            if (string.Equals(roleName, EngagementRoleConstants.TechLead, StringComparison.OrdinalIgnoreCase))
                return (int)RoleEnum.TechLead;
        }

        return null;
    }

    public RoleEnum? GetRoleEnum()
    {
        var id = GetRoleId();
        return id.HasValue && Enum.IsDefined(typeof(RoleEnum), id.Value)
            ? (RoleEnum)id.Value
            : null;
    }

    public UserRoleEnum? GetUserRole()
    {
        var id = GetRoleId();
        return id.HasValue && Enum.IsDefined(typeof(UserRoleEnum), id.Value)
            ? (UserRoleEnum)id.Value
            : null;
    }

    public string? GetEmail()
    {
        return User?.FindFirst(ClaimConstants.Email)?.Value
            ?? User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public string? GetFullName()
    {
        return User?.FindFirst(ClaimConstants.FullName)?.Value;
    }

    public bool IsInRole(string role)
    {
        if (User == null) return false;
        return User.IsInRole(role) || string.Equals(GetRole(), role, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsInRole(RoleEnum role)
    {
        return IsInRole((int)role);
    }

    public bool IsInRole(int roleId)
    {
        return GetRoleId() == roleId;
    }

    public bool IsInUserRole(UserRoleEnum role)
    {
        return GetRoleId() == (int)role;
    }

    public bool IsAdmin()
    {
        var isAdminClaim = User?.FindFirst(ClaimConstants.IsAdmin)?.Value;
        return string.Equals(isAdminClaim, "true", StringComparison.OrdinalIgnoreCase);
    }
}
