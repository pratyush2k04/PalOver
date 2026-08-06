using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            ?? throw new InvalidOperationException("No authenticated user found on the current request.");

        return int.Parse(subClaim.Value);
    }

    public string? GetRole()
    {
        return User?.FindFirst(ClaimTypes.Role)?.Value
            ?? User?.FindFirst("role")?.Value;
    }

    public int? GetRoleId()
    {
        var roleIdClaim = User?.FindFirst("roleId")?.Value
            ?? User?.FindFirst("role_id")?.Value;

        if (roleIdClaim != null && int.TryParse(roleIdClaim, out var roleId))
        {
            return roleId;
        }

        var roleName = GetRole();
        if (roleName != null)
        {
            return roleName.ToLowerInvariant() switch
            {
                "pre sales" => (int)RoleEnum.PreSales,
                "project management" => (int)RoleEnum.ProjectManagement,
                "tech lead" => (int)RoleEnum.TechLead,
                "leadership" => (int)RoleEnum.Leadership,
                _ => null
            };
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
        return User?.FindFirst("email")?.Value
            ?? User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public string? GetFullName()
    {
        return User?.FindFirst("fullname")?.Value;
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
        var isAdminClaim = User?.FindFirst("isAdmin")?.Value;
        return string.Equals(isAdminClaim, "true", StringComparison.OrdinalIgnoreCase);
    }
}
