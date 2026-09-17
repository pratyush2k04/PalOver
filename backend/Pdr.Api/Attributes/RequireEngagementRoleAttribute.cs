using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces that the authenticated caller has at least one of the specified engagement roles
/// on the target engagement (or is a System Admin).
/// Rejects unauthorized requests with 403 Forbidden.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireEngagementRoleAttribute : EngagementAuthorizationAttributeBase
{
    public EngagementRoleEnum[] RequiredRoles { get; }

    public RequireEngagementRoleAttribute(params EngagementRoleEnum[] requiredRoles)
        : base("engagementId")
    {
        RequiredRoles = requiredRoles;
    }

    public RequireEngagementRoleAttribute(string routeParameterName, params EngagementRoleEnum[] requiredRoles)
        : base(routeParameterName)
    {
        RequiredRoles = requiredRoles;
    }

    protected override async Task AuthorizeAsync(
        ActionExecutingContext context,
        IEngagementAccessService accessService,
        int engagementId)
    {
        await accessService.RequireEngagementRoleAsync(engagementId, RequiredRoles);
    }
}


