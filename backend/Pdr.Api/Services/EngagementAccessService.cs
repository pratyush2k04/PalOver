using System.Linq;
using System.Threading.Tasks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementAccessService(
    IEngagementRepository engagementRepository,
    IItemEngagementRepository itemEngagementRepository,
    ICurrentUserService currentUserService) : IEngagementAccessService
{
    public async Task<int> ResolveItemEngagementIdAsync(ItemEntityType entityType, int itemId)
    {
        if (itemId <= 0)
        {
            ThrowNotFound(entityType, itemId);
        }

        var engagementId = await itemEngagementRepository.GetEngagementIdAsync(entityType, itemId);
        if (!engagementId.HasValue || engagementId.Value <= 0)
        {
            ThrowNotFound(entityType, itemId);
        }

        return engagementId.Value;
    }

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
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
            ItemEntityType.HandoverReadinessItem => new HandoverReadinessItemNotFoundException(id),
            _ => new ItemNotFoundException(entityType.ToString(), id)
        };
    }

    public async Task ValidateAccessAsync(int engagementId)
    {
        var hasAccess = await CanAccessAsync(engagementId);
        if (!hasAccess)
        {
            throw new UserNotAssignedToEngagementException(engagementId);
        }
    }

    public async Task<bool> CanAccessAsync(int engagementId)
    {
        var userId = currentUserService.GetUserId();
        return await engagementRepository.IsUserAssignedAsync(engagementId, userId);
    }

    public async Task RequireEngagementRoleAsync(int engagementId, params EngagementRoleEnum[] requiredRoles)
    {
        var hasRole = await HasEngagementRoleAsync(engagementId, requiredRoles);
        if (!hasRole)
        {
            var roleNames = string.Join(" or ", requiredRoles.Select(r => r.ToRoleName()));
            throw new EngagementRoleAccessDeniedException($"Access denied. This action requires the '{roleNames}' engagement role on engagement {engagementId}.");
        }
    }

    public async Task<bool> HasEngagementRoleAsync(int engagementId, params EngagementRoleEnum[] roles)
    {
        var userId = currentUserService.GetUserId();
        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        var roleInts = roles.Select(r => (int)r).ToList();

        return assignments.Any(a => a.UserId == userId && roleInts.Contains(a.EngagementRoleId));
    }
}
