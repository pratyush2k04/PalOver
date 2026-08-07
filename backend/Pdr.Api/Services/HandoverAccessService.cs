using System.Threading.Tasks;
using Pdr.Api.Exceptions;
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
        var assignment = await _engagementRepository.GetAssignmentByUserAsync(engagementId, userId);

        if (assignment == null)
        {
            throw new HandoverAccessDeniedException("You don't have access to Handover.");
        }

        var roleName = assignment.User.Role.Name;
        if (roleName != "Pre Sales" && roleName != "Project Management")
        {
            throw new HandoverAccessDeniedException("You don't have access to Handover.");
        }

        return true;
    }

    public async Task<bool> CanAccessHandoverAsync(int engagementId, int userId, string requiredRole)
    {
        var assignment = await _engagementRepository.GetAssignmentByUserAsync(engagementId, userId);

        if (assignment == null)
        {
            throw new HandoverAccessDeniedException("You don't have access to Handover.");
        }

        var roleName = assignment.User.Role.Name;
        if (roleName != requiredRole)
        {
            throw new HandoverAccessDeniedException($"This action requires the '{requiredRole}' role.");
        }

        return true;
    }
}
