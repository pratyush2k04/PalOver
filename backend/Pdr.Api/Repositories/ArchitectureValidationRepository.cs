using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.ArchitectureValidation;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class ArchitectureValidationRepository(AppDbContext context) : IArchitectureValidationRepository
{
    public async Task<ArchitectureValidation?> GetByEngagementIdAsync(int engagementId) =>
        await context.ArchitectureValidations
            .Include(a => a.Features.OrderBy(f => f.CreatedAt))
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId);

    public async Task<ArchitectureValidation?> GetByIdAsync(int id) =>
        await context.ArchitectureValidations
            .Include(a => a.Features.OrderBy(f => f.CreatedAt))
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<DiscoveryFeatureItem?> GetFeatureItemByIdAsync(int featureId) =>
        await context.DiscoveryFeatureItems
            .Include(f => f.ArchitectureValidation)
            .FirstOrDefaultAsync(f => f.Id == featureId);

    public async Task AddValidationAsync(ArchitectureValidation validation) =>
        await context.ArchitectureValidations.AddAsync(validation);

    public async Task AddFeatureItemAsync(DiscoveryFeatureItem item) =>
        await context.DiscoveryFeatureItems.AddAsync(item);

    public async Task<decimal> GetCommittedTimelineWeeksAsync(int engagementId)
    {
        var handover = await context.HandoverInfos
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover is null)
        {
            return 0m;
        }

        var commitments = await context.Commitments
            .AsNoTracking()
            .Where(c => c.HandoverId == handover.HandoverId)
            .ToListAsync();

        if (commitments.Count == 0)
        {
            return 0m;
        }

        decimal totalWeeks = 0m;
        foreach (var c in commitments)
        {
            totalWeeks += c.TimelineUnit switch
            {
                CommitmentTimelineUnitEnum.Months => c.Timeline * 4m,
                CommitmentTimelineUnitEnum.Days => c.Timeline / 5m,
                _ => c.Timeline
            };
        }

        return totalWeeks;
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    public async Task<bool> EngagementExistsAsync(int engagementId) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);
}
