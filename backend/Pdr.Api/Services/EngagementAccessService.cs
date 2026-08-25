using System.Threading.Tasks;
using Pdr.Api.Exceptions;
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
        var userId = currentUserService.GetUserId();
        return await engagementRepository.IsUserAssignedAsync(engagementId, userId);
    }
}
