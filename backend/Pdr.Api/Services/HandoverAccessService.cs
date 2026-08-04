using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Exceptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverAccessService : IHandoverAccessService
{
    private readonly AppDbContext _context;

    public HandoverAccessService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanAccessHandoverAsync(long engagementId, int userId)
    {
        var assignment = await _context.EngagementAssignments
            .Include(ea => ea.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(ea => ea.EngagementId == engagementId && ea.UserId == userId);

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

    public async Task<bool> CanAccessHandoverAsync(long engagementId, int userId, string requiredRole)
    {
        var assignment = await _context.EngagementAssignments
            .Include(ea => ea.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(ea => ea.EngagementId == engagementId && ea.UserId == userId);

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
