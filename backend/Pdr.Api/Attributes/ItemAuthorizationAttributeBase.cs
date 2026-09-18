using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Data;
using Pdr.Api.Exceptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Supported entity types for item-level engagement authorization.
/// </summary>
public enum ItemEntityType
{
    AccessItem,
    EnvironmentTooling,
    ChangeRequest,
    BrownfieldFinding,
    DiscoveryFeature,
    Document
}

/// <summary>
/// Abstract base class for action filters that enforce item-level engagement authorization.
/// Resolves the parent engagement ID by looking up the item by ID in the database before the action method executes.
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
        var itemId = ExtractItemId(context, RouteParameterName);
        if (itemId <= 0)
        {
            ThrowNotFound(EntityType, itemId);
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var engagementAccessService = context.HttpContext.RequestServices.GetRequiredService<IEngagementAccessService>();

        int engagementId = await ResolveEngagementIdAsync(db, EntityType, itemId);

        if (engagementId <= 0)
        {
            ThrowNotFound(EntityType, itemId);
        }

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

    private static async Task<int> ResolveEngagementIdAsync(AppDbContext db, ItemEntityType entityType, int itemId)
    {
        return entityType switch
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

            ItemEntityType.Document => await db.Documents
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => x.EngagementId)
                .FirstOrDefaultAsync(),

            _ => 0
        };
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
            ItemEntityType.Document => new DocumentNotFoundException(id),
            _ => new ItemNotFoundException(entityType.ToString(), id)
        };
    }
}
