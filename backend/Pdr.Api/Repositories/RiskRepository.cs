using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
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

    public async Task<Risk?> GetByIdAsync(int id)
    {
        return await context.Risks
            .Include(r => r.Owner)
                .ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(r => r.Id == id);
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
}
