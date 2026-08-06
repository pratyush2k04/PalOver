using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverAccessService : IHandoverAccessService
{
    private readonly AppDbContext _context;

    public HandoverAccessService(AppDbContext context)
    {
        _context = context;
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
}
