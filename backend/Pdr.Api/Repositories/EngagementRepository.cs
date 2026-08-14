using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class EngagementRepository(AppDbContext context) : IEngagementRepository
{
    public async Task<List<Engagement>> GetEngagementsForUserAsync(int userId, string sortBy)
    {
        var query = context.Engagements
            .AsNoTracking()
            .Include(e => e.EngagementType)
            .Include(e => e.CommercialModel)
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId));

        query = sortBy switch
        {
            "client" => query.OrderBy(e => e.ClientName),
            _ => query.OrderBy(e => e.KickoffDate)
        };

        return await query.ToListAsync();
    }

    public async Task<Engagement?> GetByIdAsync(int engagementId) =>
        await context.Engagements
            .Include(e => e.EngagementType)
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

        //for assigning
    public async Task<List<EngagementAssignment>> GetAssignmentsAsync(int engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Where(a => a.EngagementId == engagementId)
            .ToListAsync();

    public async Task<List<EngagementAssignment>> GetLeadershipAssignmentsAsync(int engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Where(a => a.EngagementId == engagementId && a.User.RoleId == (int)RoleEnum.Leadership)
            .ToListAsync();

    public async Task<EngagementAssignment?> GetAssignmentByRoleIdAsync(int engagementId, int roleId) =>
        await context.EngagementAssignments
            .Include(a => a.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.User.RoleId == roleId);

    public async Task<EngagementAssignment?> GetAssignmentByUserAsync(int engagementId, int userId) =>
        await context.EngagementAssignments
            .Include(a => a.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.UserId == userId);

    public async Task RemoveAssignmentAsync(EngagementAssignment assignment)
    {
        context.EngagementAssignments.Remove(assignment);
        await context.SaveChangesAsync();
    }

    public async Task AddAssignmentAsync(int engagementId, int userId)
    {
        context.EngagementAssignments.Add(new EngagementAssignment
        {
            EngagementId = engagementId,
            UserId = userId
        });
        await context.SaveChangesAsync();
    }
}
