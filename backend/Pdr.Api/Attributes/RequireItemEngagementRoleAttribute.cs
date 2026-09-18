using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces that the caller has one of the required engagement roles for the engagement that owns the target item entity.
/// Resolves the engagement ID by looking up the item by ID in the database before the action method executes.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireItemEngagementRoleAttribute : ItemAuthorizationAttributeBase
{
    public EngagementRoleEnum[] RequiredRoles { get; }

    public RequireItemEngagementRoleAttribute(ItemEntityType entityType, params EngagementRoleEnum[] requiredRoles)
        : base(entityType)
    {
        RequiredRoles = requiredRoles;
    }

    public RequireItemEngagementRoleAttribute(ItemEntityType entityType, string routeParameterName, params EngagementRoleEnum[] requiredRoles)
        : base(entityType, routeParameterName)
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
