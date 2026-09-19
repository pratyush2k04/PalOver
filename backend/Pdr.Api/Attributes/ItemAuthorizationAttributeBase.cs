using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Abstract base class for action filters that enforce item-level engagement authorization.
/// Resolves the parent engagement ID via the service layer before the action method executes.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class ItemAuthorizationAttributeBase : Attribute, IAsyncActionFilter
{
    public ItemEntityType EntityType { get; }
    public string RouteParameterName { get; set; }

    protected ItemAuthorizationAttributeBase(ItemEntityType entityType, string? routeParameterName = null)
    {
        EntityType = entityType;
        RouteParameterName = routeParameterName ?? entityType switch
        {
            ItemEntityType.BrownfieldFinding => "findingId",
            ItemEntityType.DiscoveryFeature => "featureId",
            _ => "id"
        };
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var allowsAdmin = context.ActionDescriptor.EndpointMetadata.OfType<AllowAdminAttribute>().Any();
        if (allowsAdmin)
        {
            var currentUserService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
            if (currentUserService.IsAdmin())
            {
                await next();
                return;
            }
        }

        var itemId = ExtractItemId(context, RouteParameterName);
        var engagementAccessService = context.HttpContext.RequestServices.GetRequiredService<IEngagementAccessService>();

        int engagementId = await engagementAccessService.ResolveItemEngagementIdAsync(EntityType, itemId);

        await AuthorizeAsync(context, engagementAccessService, engagementId);

        await next();
    }

    /// <summary>
    /// Performs the specific authorization check for the resolved engagement ID.
    /// </summary>
    protected abstract Task AuthorizeAsync(
        ActionExecutingContext context,
        IEngagementAccessService accessService,
        int engagementId);

    private static int ExtractItemId(ActionExecutingContext context, string routeParam)
    {
        if (context.RouteData.Values.TryGetValue(routeParam, out var val) && val is not null)
        {
            if (int.TryParse(val.ToString(), out var id) && id > 0)
            {
                return id;
            }
        }

        // Fallback checks
        foreach (var key in new[] { "id", "findingId", "featureId" })
        {
            if (context.RouteData.Values.TryGetValue(key, out var fallbackVal) && fallbackVal is not null)
            {
                if (int.TryParse(fallbackVal.ToString(), out var id) && id > 0)
                {
                    return id;
                }
            }
        }

        return 0;
    }
}
