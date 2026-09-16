using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverAccessService : IHandoverAccessService
{
    private readonly IEngagementRepository _engagementRepository;

    public HandoverAccessService(IEngagementRepository engagementRepository)
    {
        _engagementRepository = engagementRepository;
    }

    public async Task<bool> CanAccessHandoverAsync(int engagementId, int userId)
    {
        var assignments = await _engagementRepository.GetAssignmentsAsync(engagementId);
        if (!assignments.Any(a => a.UserId == userId))
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.AccessDenied);
        }

        return true;
    }

    public async Task<bool> CanAccessHandoverAsync(int engagementId, int userId, string requiredRole)
    {
        var assignments = await _engagementRepository.GetAssignmentsAsync(engagementId);
        var userAssignments = assignments.Where(a => a.UserId == userId).ToList();

        if (userAssignments.Count == 0)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.AccessDenied);
        }

        var hasRole = userAssignments.Any(a =>
            (a.EngagementRole != null && string.Equals(a.EngagementRole.Name, requiredRole, StringComparison.OrdinalIgnoreCase)) ||
            string.Equals(((EngagementRoleEnum)a.EngagementRoleId).ToRoleName(), requiredRole, StringComparison.OrdinalIgnoreCase) ||
            (a.User?.Role != null && string.Equals(a.User.Role.Name, requiredRole, StringComparison.OrdinalIgnoreCase)));

        if (!hasRole)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.ActionRequiresRole(requiredRole));
        }

        return true;
    }

    public async Task<bool> CanAccessHandoverAsync(int engagementId, int userId, params string[] requiredRoles)
    {
        var assignments = await _engagementRepository.GetAssignmentsAsync(engagementId);
        var userAssignments = assignments.Where(a => a.UserId == userId).ToList();

        if (userAssignments.Count == 0)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.AccessDenied);
        }

        var hasRole = userAssignments.Any(a =>
            (a.EngagementRole != null && requiredRoles.Any(r => string.Equals(r, a.EngagementRole.Name, StringComparison.OrdinalIgnoreCase))) ||
            requiredRoles.Any(r => string.Equals(r, ((EngagementRoleEnum)a.EngagementRoleId).ToRoleName(), StringComparison.OrdinalIgnoreCase)) ||
            (a.User?.Role != null && requiredRoles.Any(r => string.Equals(r, a.User.Role.Name, StringComparison.OrdinalIgnoreCase))));

        if (!hasRole)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.ActionRequiresAnyRole(requiredRoles));
        }

        return true;
    }
}
