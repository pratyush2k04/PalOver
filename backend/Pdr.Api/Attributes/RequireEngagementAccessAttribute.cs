using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Exceptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces that the authenticated caller is assigned to the specified engagement (or is a System Admin).
/// Rejects unauthorized requests with 403 Forbidden.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireEngagementAccessAttribute : Attribute, IAsyncActionFilter
{
    public string RouteParameterName { get; }

    public RequireEngagementAccessAttribute(string routeParameterName = "engagementId")
    {
        RouteParameterName = routeParameterName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var engagementId = ExtractEngagementId(context, RouteParameterName);

        var engagementAccessService = context.HttpContext.RequestServices.GetRequiredService<IEngagementAccessService>();
        await engagementAccessService.ValidateAccessAsync(engagementId);

        await next();
    }

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
