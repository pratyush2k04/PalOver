using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Models;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Models.Risks;
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
            .Include(e => e.DayOneSignOffRecord)
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId));

        query = sortBy switch
        {
            "client" => query.OrderBy(e => e.ClientName),
            _ => query.OrderBy(e => e.KickoffDate)
        };

        return await query.ToListAsync();
    }

    public async Task<List<int>> GetEngagementIdsForUserAsync(int userId) =>
        await context.Engagements
            .AsNoTracking()
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId))
            .Select(e => e.EngagementId)
            .ToListAsync();

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

    public async Task<EngagementOpenItemsSummaryDto> GetOpenItemsSummaryAsync(int engagementId)
    {
        var checklistOpenCount = await context.Set<EngagementChecklistItem>()
            .AsNoTracking()
            .Where(i => i.EngagementChecklist.EngagementId == engagementId
                        && i.IsMandatory
                        && i.IsApplicable
                        && i.Status == "Incomplete")
            .CountAsync();

        var accessOpenCount = await context.Set<AccessItem>()
            .AsNoTracking()
            .Where(a => a.EngagementId == engagementId
                        && a.Status != AccessItemStatus.Granted)
            .CountAsync();

        var openRisksCount = await context.Set<Risk>()
            .AsNoTracking()
            .Where(r => r.EngagementId == engagementId
                        && r.Status != RiskStatus.Closed)
            .CountAsync();

        var resourcingOpenCount = await context.Set<ResourceRequest>()
            .AsNoTracking()
            .Where(r => r.EngagementId == engagementId
                        && r.Status != ResourceRequestStatus.Confirmed)
            .CountAsync();

        return new EngagementOpenItemsSummaryDto
        {
            ChecklistOpenCount = checklistOpenCount,
            AccessOpenCount = accessOpenCount,
            OpenRisksCount = openRisksCount,
            ResourcingOpenCount = resourcingOpenCount
        };
    }

    public async Task<Dictionary<int, Engagement>> GetByIdsAsync(IEnumerable<int> engagementIds) =>
        await context.Engagements
            .Where(e => engagementIds.Contains(e.EngagementId) && !e.IsDeleted)
            .ToDictionaryAsync(e => e.EngagementId);
}
