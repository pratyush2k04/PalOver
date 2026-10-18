using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Constants;
using Pdr.Api.Data;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Models.HandoverReadiness;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class RiskRepository(AppDbContext context) : IRiskRepository
{
    public async Task<List<Risk>> GetByEngagementIdAsync(int engagementId, RiskStatus? status = null, int? ownerId = null)
    {
        var query = context.Risks
            .Include(r => r.Owner)
                .ThenInclude(u => u!.Role)
            .Where(r => r.EngagementId == engagementId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (ownerId.HasValue)
        {
            query = query.Where(r => r.OwnerId == ownerId.Value);
        }

        return await query
            .OrderByDescending(r => (int)r.Impact * (int)r.Likelihood)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Risk>> GetOpenRiskBlockersAsync(int engagementId)
    {
        return await context.Risks
            .AsNoTracking()
            .Include(r => r.Owner)
            .Where(r => r.EngagementId == engagementId &&
                        (r.Status == RiskStatus.Open || r.Status == RiskStatus.Mitigating))
            .ToListAsync();
    }

    public async Task<Risk?> GetByIdAsync(int id)
    {
        return await context.Risks
            .Include(r => r.Owner)
                .ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Dictionary<int, Risk>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new();

        return await context.Risks
            .AsNoTracking()
            .Where(r => idList.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id);
    }

    public async Task AddAsync(Risk risk)
    {
        await context.Risks.AddAsync(risk);
    }

    public Task UpdateAsync(Risk risk)
    {
        context.Risks.Update(risk);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<List<RiskBlockerSourceItem>> GetActiveAccessBlockersAsync(int engagementId)
    {
        var engagement = await context.Engagements
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId && !e.IsDeleted);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int? daysToBilling = engagement?.BillingStartDate.HasValue == true
            ? engagement.BillingStartDate.Value.DayNumber - today.DayNumber
            : null;

        var isBillingApproaching = daysToBilling.HasValue && daysToBilling.Value < 3;

        return await context.AccessItems
            .AsNoTracking()
            .Where(a => a.EngagementId == engagementId &&
                        !a.IsDeleted &&
                        (a.Status == AccessItemStatus.Blocked ||
                         (isBillingApproaching && a.IsClientSide && a.Status != AccessItemStatus.Granted)))
            .Select(a => new RiskBlockerSourceItem(
                RiskConstants.SourceTypes.AccessTracker,
                a.Id,
                a.Name,
                a.Status == AccessItemStatus.Blocked
                    ? (a.BlockReason ?? "Access request is blocked")
                    : $"Client-side access request is overdue ({daysToBilling} days to billing).",
                (int?)a.OwnerId))
            .ToListAsync();
    }

    public async Task<List<RiskBlockerSourceItem>> GetActiveToolingBlockersAsync(int engagementId)
    {
        return await context.EnvironmentToolingItems
            .AsNoTracking()
            .Include(e => e.EnvironmentToolingDefinition)
            .Where(e => e.EngagementId == engagementId &&
                        e.Status == EnvironmentToolingStatus.Blocked &&
                        !e.IsDeleted)
            .Select(e => new RiskBlockerSourceItem(
                RiskConstants.SourceTypes.EnvironmentTooling,
                e.Id,
                e.EnvironmentToolingDefinition != null ? e.EnvironmentToolingDefinition.Name : (e.CustomName ?? e.Name),
                e.BlockReason ?? string.Empty,
                (int?)e.OwnerId))
            .ToListAsync();
    }

    public async Task<List<RiskBlockerSourceItem>> GetActiveChecklistBlockersAsync(int engagementId)
    {
        return await context.EngagementChecklistItems
            .AsNoTracking()
            .Where(c => c.EngagementChecklist.EngagementId == engagementId &&
                        c.Status == MasterChecklistConstants.ItemStatuses.Blocked &&
                        c.IsApplicable &&
                        !c.IsDeleted)
            .Select(c => new RiskBlockerSourceItem(
                RiskConstants.SourceTypes.Checklist,
                c.Id,
                c.Title,
                c.BlockReason ?? string.Empty,
                (int?)c.OwnerId))
            .ToListAsync();
    }

    public async Task<List<RiskBlockerSourceItem>> GetActiveHandoverReadinessBlockersAsync(int engagementId)
    {
        return await context.HandoverReadinessItems
            .AsNoTracking()
            .Where(h => h.EngagementId == engagementId &&
                        h.Status == HandoverReadinessStatus.Blocked &&
                        !h.IsDeleted)
            .Select(h => new RiskBlockerSourceItem(
                RiskConstants.SourceTypes.HandoverReadiness,
                h.Id,
                h.Name,
                h.BlockReason ?? "Handover readiness item is blocked",
                (int?)h.OwnerId))
            .ToListAsync();
    }

    public async Task<RiskBlockerSourceItem?> GetActiveResourceGapBlockerAsync(int engagementId, int thresholdDays)
    {
        var engagement = await context.Engagements
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId && !e.IsDeleted);

        if (engagement?.BillingStartDate == null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysToBilling = engagement.BillingStartDate.Value.DayNumber - today.DayNumber;

        if (daysToBilling > thresholdDays)
        {
            return null;
        }

        var unconfirmedCount = await context.ResourceRequests
            .AsNoTracking()
            .Where(r => r.EngagementId == engagementId &&
                        r.Status != ResourceRequestStatus.Confirmed &&
                        !r.IsDeleted)
            .CountAsync();

        if (unconfirmedCount == 0)
        {
            return null;
        }

        return new RiskBlockerSourceItem(
            RiskConstants.SourceTypes.ResourceRequest,
            engagementId,
            "Resources not confirmed",
            $"{unconfirmedCount} resource request(s) not confirmed with {daysToBilling} day(s) until billing starts ({engagement.BillingStartDate.Value:dd MMM yyyy}).",
            null);
    }

    public async Task<List<Risk>> GetAutoCreatedRisksAsync(int engagementId)
    {
        return await context.Risks
            .Where(r => r.EngagementId == engagementId &&
                        r.SourceType != null &&
                        r.SourceItemId != null)
            .ToListAsync();
    }

    public async Task<List<Risk>> GetOpenRisksAsync()
    {
        return await context.Risks
            .Where(r => r.Status != RiskStatus.Closed && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<RiskEscalationHistory>> GetEscalationHistoriesByRiskIdsAsync(IEnumerable<int> riskIds)
    {
        return await context.RiskEscalationHistories
            .Where(h => riskIds.Contains(h.RiskId) && !h.IsDeleted)
            .OrderByDescending(h => h.EscalatedAt)
            .ToListAsync();
    }

    public async Task<List<RiskEscalationHistory>> GetEscalationHistoryByRiskIdAsync(int riskId)
    {
        return await context.RiskEscalationHistories
            .Include(h => h.EscalatedToUser)
            .Where(h => h.RiskId == riskId && !h.IsDeleted)
            .OrderByDescending(h => h.EscalatedAt)
            .ToListAsync();
    }

    public async Task AddEscalationHistoryAsync(RiskEscalationHistory history)
    {
        await context.RiskEscalationHistories.AddAsync(history);
    }}
