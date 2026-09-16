using System.Linq;
using System.Threading.Tasks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementAccessService(
    IEngagementRepository engagementRepository,
    ICurrentUserService currentUserService) : IEngagementAccessService
{
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
        if (currentUserService.IsAdmin() || currentUserService.IsInUserRole(UserRoleEnum.Leadership)) return true;
        var userId = currentUserService.GetUserId();
        return await engagementRepository.IsUserAssignedAsync(engagementId, userId);
    }

    public async Task RequireEngagementRoleAsync(int engagementId, params EngagementRoleEnum[] requiredRoles)
    {
        if (currentUserService.IsAdmin()) return;

        var hasRole = await HasEngagementRoleAsync(engagementId, requiredRoles);
        if (!hasRole)
        {
            var roleNames = string.Join(" or ", requiredRoles.Select(r => r.ToRoleName()));
            throw new EngagementRoleAccessDeniedException($"Access denied. This action requires the '{roleNames}' engagement role on engagement {engagementId}.");
        }
    }

    public async Task<bool> HasEngagementRoleAsync(int engagementId, params EngagementRoleEnum[] roles)
    {
        if (currentUserService.IsAdmin()) return true;

        if (roles.Contains(EngagementRoleEnum.Leadership) && currentUserService.IsInUserRole(UserRoleEnum.Leadership))
        {
            return true;
        }

        var userId = currentUserService.GetUserId();
        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        var roleInts = roles.Select(r => (int)r).ToList();

        return assignments.Any(a => a.UserId == userId && roleInts.Contains(a.EngagementRoleId));
    }
}
