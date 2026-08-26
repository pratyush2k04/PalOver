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
        var assignment = await _engagementRepository.GetAssignmentByUserAsync(engagementId, userId);

        if (assignment == null)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.AccessDenied);
        }

        return true;
    }

    public async Task<bool> CanAccessHandoverAsync(int engagementId, int userId, string requiredRole)
    {
        var assignment = await _engagementRepository.GetAssignmentByUserAsync(engagementId, userId);

        if (assignment == null)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.AccessDenied);
        }

        var roleName = assignment.User.Role.Name;
        if (roleName != requiredRole)
        {
            throw new HandoverAccessDeniedException(HandoverValidationConstants.ActionRequiresRole(requiredRole));
        }

        return true;
    }
}
