using System.Threading.Tasks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
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
        var assignment = await _context.EngagementAssignments
            .Include(ea => ea.User)
            .FirstOrDefaultAsync(ea => ea.EngagementId == engagementId && ea.UserId == userId);


        if (assignment == null)
        {
            throw new HandoverAccessDeniedException("You don't have access to Handover.");
        }

        var roleId = assignment.User.RoleId;
        if (roleId != (int)RoleEnum.PreSales && roleId != (int)RoleEnum.ProjectManagement)
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
