using Microsoft.EntityFrameworkCore;
using Pdr.Api.Constants;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HomeRepository(AppDbContext db) : IHomeRepository
{
    public async Task<User?> GetUserWithRoleAsync(int userId)
    {
        return await db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<List<int>> GetUserEngagementIdsAsync(int userId)
    {
        return await db.EngagementAssignments
            .Where(ea => ea.UserId == userId)
            .Select(ea => ea.EngagementId)
            .ToListAsync();
    }

    public async Task<int> GetDraftHandoversCountAsync(List<int> engagementIds)
    {
        return await db.HandoverInfos
            .Where(h => engagementIds.Contains(h.EngagementId) 
                     && h.StatusId == (int)HandoverStatusEnum.Draft)
            .CountAsync();
    }

    public async Task<int> GetCompleteHandoversCountAsync(List<int> engagementIds)
    {
        return await db.HandoverInfos
            .Where(h => engagementIds.Contains(h.EngagementId) 
                     && h.StatusId == (int)HandoverStatusEnum.Submitted)
            .CountAsync();
    }

    public async Task<int> GetIncompleteChecklistCountAsync(List<int> engagementIds)
    {
        return await db.EngagementChecklistItems
            .Where(c => engagementIds.Contains(c.EngagementChecklist.EngagementId) 
                     && c.Status == HomeConstants.Statuses.Incomplete 
                     && c.IsMandatory)
            .CountAsync();
    }

    public async Task<int> GetOpenBlockersCountAsync(List<int> engagementIds)
    {
        return await db.Risks
            .Where(r => engagementIds.Contains(r.EngagementId) 
                     && r.Status == RiskStatus.Open)
            .CountAsync();
    }

    public async Task<int> GetOpenAccessItemsCountAsync(List<int> engagementIds)
    {
        return await db.AccessItems
            .Where(a => engagementIds.Contains(a.EngagementId) 
                     && a.Status == AccessItemStatus.InProgress)
            .CountAsync();
    }

    public async Task<int> GetMyEscalationsCountAsync(int userId)
    {
        return await db.RiskEscalationHistories
            .Where(e => e.EscalatedToUserId == userId)
            .CountAsync();
    }

    public async Task<List<DashboardEngagementDto>> GetUpcomingKickoffsAsync(List<int> engagementIds, DateOnly cutoffDate)
    {
        return await db.Engagements
            .Where(e => engagementIds.Contains(e.EngagementId) && e.KickoffDate <= cutoffDate)
            .OrderBy(e => e.KickoffDate)
            .Select(e => new DashboardEngagementDto
            {
                EngagementId = e.EngagementId,
                EngagementName = e.EngagementName,
                KickoffDate = e.KickoffDate
            })
            .ToListAsync();
    }

    public async Task<int> GetEngagementOpenBlockerCountAsync(int engagementId)
    {
        return await db.Risks
            .CountAsync(r => r.EngagementId == engagementId &&
                             (r.Status == RiskStatus.Open || r.Status == RiskStatus.Mitigating) &&
                             !r.IsDeleted);
    }
}
