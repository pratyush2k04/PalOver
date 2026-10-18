using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces that the authenticated caller is assigned to the engagement that owns the target item.
/// Rejects unauthorized requests with 403 Forbidden.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireItemEngagementAccessAttribute : ItemAuthorizationAttributeBase
{
    public RequireItemEngagementAccessAttribute(ItemEntityType entityType, string? routeParameterName = null)
        : base(entityType, routeParameterName)
    {
    }

    protected override async Task AuthorizeAsync(
        ActionExecutingContext context,
        IEngagementAccessService accessService,
        int engagementId)
    {
        await accessService.ValidateAccessAsync(engagementId);
    }
}
