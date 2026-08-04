using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class EngagementRepository(AppDbContext context) : IEngagementRepository
{
    public async Task<List<Engagement>> GetEngagementsForPmAsync(int userId, string sortBy)
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
        //for assigning 
    public async Task<List<EngagementAssignment>> GetAssignmentsAsync(long engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Where(a => a.EngagementId == engagementId)
            .ToListAsync();

    public async Task<EngagementAssignment?> GetAssignmentByRoleAsync(long engagementId, string roleName) =>
        await context.EngagementAssignments
            .Include(a => a.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.User.Role.Name == roleName);

    public async Task RemoveAssignmentAsync(EngagementAssignment assignment)
    {
        context.EngagementAssignments.Remove(assignment);
        await context.SaveChangesAsync();
    }

    public async Task AddAssignmentAsync(long engagementId, int userId)
    {
        context.EngagementAssignments.Add(new EngagementAssignment
        {
            EngagementId = (int)engagementId,
            UserId = userId
        });
        await context.SaveChangesAsync();
    }
}
