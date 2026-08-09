using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Pdr.Api.Constants;
using Pdr.Api.Models;
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
            if (string.Equals(roleName, RoleConstants.PreSales, StringComparison.OrdinalIgnoreCase))
                return (int)RoleEnum.PreSales;
            if (string.Equals(roleName, RoleConstants.ProjectManagement, StringComparison.OrdinalIgnoreCase))
                return (int)RoleEnum.ProjectManagement;
            if (string.Equals(roleName, RoleConstants.TechLead, StringComparison.OrdinalIgnoreCase))
                return (int)RoleEnum.TechLead;
            if (string.Equals(roleName, RoleConstants.Leadership, StringComparison.OrdinalIgnoreCase))
                return (int)RoleEnum.Leadership;
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

    public bool IsAdmin()
    {
        var isAdminClaim = User?.FindFirst(ClaimConstants.IsAdmin)?.Value;
        return string.Equals(isAdminClaim, "true", StringComparison.OrdinalIgnoreCase);
    }
}
