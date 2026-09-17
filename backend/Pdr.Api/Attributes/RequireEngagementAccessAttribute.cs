using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces that the authenticated caller is assigned to the specified engagement (or is a System Admin).
/// Rejects unauthorized requests with 403 Forbidden.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireEngagementAccessAttribute : EngagementAuthorizationAttributeBase
{
    public RequireEngagementAccessAttribute(string routeParameterName = "engagementId")
        : base(routeParameterName)
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


