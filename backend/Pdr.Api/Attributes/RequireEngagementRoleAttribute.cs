using System;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces that the authenticated caller has at least one of the specified engagement roles
/// on the target engagement (or is a System Admin).
/// Rejects unauthorized requests with 403 Forbidden.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireEngagementRoleAttribute : RequireEngagementAccessAttribute
{
    public RequireEngagementRoleAttribute(params EngagementRoleEnum[] requiredRoles)
        : base(requiredRoles)
    {
    }

    public RequireEngagementRoleAttribute(string routeParameterName, params EngagementRoleEnum[] requiredRoles)
        : base(routeParameterName, requiredRoles)
    {
    }
}

