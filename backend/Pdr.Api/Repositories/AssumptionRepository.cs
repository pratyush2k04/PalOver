using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class AssumptionRepository(AppDbContext context) : IAssumptionRepository
{
    public async Task<List<Assumption>> GetByEngagementIdAsync(int engagementId)
    {
        return await context.Assumptions
            .Include(a => a.Owner)
            .Where(a => a.EngagementId == engagementId)
            .ToListAsync();
    }

    public async Task<Assumption?> GetByIdAsync(int id)
    {
        return await context.Assumptions
            .Include(a => a.Owner)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Dictionary<int, Assumption>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<int, Assumption>();

        return await context.Assumptions
            .AsNoTracking()
            .Where(a => idList.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }

    public async Task<Assumption> AddAsync(Assumption assumption)
    {
        context.Assumptions.Add(assumption);
        await context.SaveChangesAsync();
        return assumption;
    }

    public async Task<Assumption> UpdateAsync(Assumption assumption)
    {
        context.Assumptions.Update(assumption);
        await context.SaveChangesAsync();
        return assumption;
    }

    public async Task DeleteAsync(Assumption assumption)
    {
        context.Assumptions.Remove(assumption);
        await context.SaveChangesAsync();
    }

    public async Task<int> GetUnvalidatedCountByEngagementIdAsync(int engagementId)
    {
        return await context.Assumptions
            .CountAsync(a => a.EngagementId == engagementId && a.Status == AssumptionStatus.Unvalidated);
    }

    public async Task<int> GetInvalidWithoutRiskCountByEngagementIdAsync(int engagementId)
    {
        return await context.Assumptions
            .CountAsync(a => a.EngagementId == engagementId && a.Status == AssumptionStatus.Invalid && a.LinkedRiskId == null);
    }}
