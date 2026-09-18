using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Exceptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Abstract base class for action filters that enforce engagement-level authorization.
/// Handles route/action parameter extraction and provides a template method for specific authorization checks.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class EngagementAuthorizationAttributeBase : Attribute, IAsyncActionFilter
{
    public string RouteParameterName { get; set; }

    protected EngagementAuthorizationAttributeBase(string routeParameterName = "engagementId")
    {
        RouteParameterName = routeParameterName;
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

        var engagementId = ExtractEngagementId(context, RouteParameterName);
        var accessService = context.HttpContext.RequestServices.GetRequiredService<IEngagementAccessService>();

        await AuthorizeAsync(context, accessService, engagementId);

        await next();
    }

    /// <summary>
    /// Performs the specific authorization check for the resolved engagement ID.
    /// </summary>
    protected abstract Task AuthorizeAsync(
        ActionExecutingContext context,
        IEngagementAccessService accessService,
        int engagementId);

    private static int ExtractEngagementId(ActionExecutingContext context, string routeParam)
    {
        if (context.RouteData.Values.TryGetValue(routeParam, out var val) && val is not null)
        {
            if (int.TryParse(val.ToString(), out var id) && id > 0)
            {
                return id;
            }
        }

        var fallbackKey = string.Equals(routeParam, "id", StringComparison.OrdinalIgnoreCase) ? "engagementId" : "id";
        if (context.RouteData.Values.TryGetValue(fallbackKey, out var fallbackVal) && fallbackVal is not null)
        {
            if (int.TryParse(fallbackVal.ToString(), out var id) && id > 0)
            {
                return id;
            }
        }

        if (context.ActionArguments.TryGetValue(routeParam, out var argVal) && argVal is int argInt && argInt > 0)
        {
            return argInt;
        }

        if (context.ActionArguments.TryGetValue(fallbackKey, out var argFallback) && argFallback is int argFallbackInt && argFallbackInt > 0)
        {
            return argFallbackInt;
        }

        throw new EngagementNotFoundException(0);
    }
}
