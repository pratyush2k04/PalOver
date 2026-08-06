using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Risk;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class RiskRepository(AppDbContext context) : IRiskRepository
{
    public async Task<List<Risk>> GetByEngagementIdAsync(int engagementId) =>
        await context.Risks
            .AsNoTracking()
            .Where(r => r.EngagementId == engagementId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Risk?> GetByIdAsync(int riskId) =>
        await context.Risks
            .FirstOrDefaultAsync(r => r.RiskId == riskId);

    public async Task<Risk?> GetOpenByTitleAsync(int engagementId, string title) =>
        await context.Risks
            .FirstOrDefaultAsync(r => r.EngagementId == engagementId &&
                                      r.Title == title &&
                                      r.Status == RiskStatus.Open);

    public async Task<Risk?> GetOpenRiskAsync(int engagementId, string title, int? accessItemId = null, int? environmentToolingItemId = null)
    {
        var query = context.Risks.Where(r => r.EngagementId == engagementId && r.Status == RiskStatus.Open);

        if (accessItemId.HasValue)
        {
            query = query.Where(r => r.AccessItemId == accessItemId.Value);
        }
        else if (environmentToolingItemId.HasValue)
        {
            query = query.Where(r => r.EnvironmentToolingItemId == environmentToolingItemId.Value);
        }
        else
        {
            query = query.Where(r => r.Title == title);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task AddAsync(Risk risk)
    {
        await context.Risks.AddAsync(risk);
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    public async Task<bool> EngagementExistsAsync(int engagementId) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);
}
