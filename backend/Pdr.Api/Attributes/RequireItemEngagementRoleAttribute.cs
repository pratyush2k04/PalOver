using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Data;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Supported entity types for item-level engagement role authorization.
/// </summary>
public enum ItemEntityType
{
    AccessItem,
    EnvironmentTooling,
    ChangeRequest,
    BrownfieldFinding,
    DiscoveryFeature
}

/// <summary>
/// Enforces that the caller has one of the required engagement roles for the engagement that owns the target item entity.
/// Resolves the engagement ID by looking up the item by ID in the database before the action method executes.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireItemEngagementRoleAttribute : Attribute, IAsyncActionFilter
{
    public ItemEntityType EntityType { get; }
    public EngagementRoleEnum[] RequiredRoles { get; }
    public string RouteParameterName { get; set; }

    public RequireItemEngagementRoleAttribute(ItemEntityType entityType, params EngagementRoleEnum[] requiredRoles)
    {
        EntityType = entityType;
        RequiredRoles = requiredRoles;
        RouteParameterName = entityType switch
        {
            ItemEntityType.BrownfieldFinding => "findingId",
            ItemEntityType.DiscoveryFeature => "featureId",
            _ => "id"
        };
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var itemId = ExtractItemId(context, RouteParameterName);
        if (itemId <= 0)
        {
            ThrowNotFound(EntityType, itemId);
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var engagementAccessService = context.HttpContext.RequestServices.GetRequiredService<IEngagementAccessService>();

        int engagementId = EntityType switch
        {
            ItemEntityType.AccessItem => await db.AccessItems
                .Where(x => x.Id == itemId)
                .Select(x => x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.EnvironmentTooling => await db.EnvironmentToolingItems
                .Where(x => x.Id == itemId)
                .Select(x => x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.ChangeRequest => await db.ChangeRequests
                .Where(x => x.Id == itemId)
                .Select(x => x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.BrownfieldFinding => await db.BrownfieldFindings
                .Where(x => x.Id == itemId)
                .Select(x => x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.DiscoveryFeature => await db.DiscoveryFeatureItems
                .Where(x => x.Id == itemId)
                .Select(x => x.ArchitectureValidation != null ? x.ArchitectureValidation.EngagementId : 0)
                .FirstOrDefaultAsync(),

            _ => 0
        };

        if (engagementId <= 0)
        {
            ThrowNotFound(EntityType, itemId);
        }

        await engagementAccessService.RequireEngagementRoleAsync(engagementId, RequiredRoles);

        await next();
    }

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

    private static void ThrowNotFound(ItemEntityType entityType, int id)
    {
        throw entityType switch
        {
            ItemEntityType.AccessItem => new AccessItemNotFoundException(id),
            ItemEntityType.EnvironmentTooling => new EnvironmentToolingItemNotFoundException(id),
            ItemEntityType.ChangeRequest => new ChangeRequestNotFoundException(id),
            ItemEntityType.BrownfieldFinding => new BrownfieldFindingNotFoundException(id),
            ItemEntityType.DiscoveryFeature => new DiscoveryFeatureItemNotFoundException(id),
            _ => new InvalidOperationException($"Item with ID {id} not found.")
        };
    }
}
