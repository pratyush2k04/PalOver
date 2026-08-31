using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class EnvironmentToolingRepository(AppDbContext context) : IEnvironmentToolingRepository
{
    public async Task<List<EnvironmentToolingItem>> GetByEngagementIdAsync(int engagementId) =>
        await context.EnvironmentToolingItems
            .AsNoTracking()
            .Include(e => e.Owner)
            .Include(e => e.EnvironmentToolingDefinition)
            .Where(e => e.EngagementId == engagementId)
            .OrderBy(e => e.EnvironmentToolingDefinition != null ? e.EnvironmentToolingDefinition.DisplayOrder : int.MaxValue)
            .ThenBy(e => e.Id)
            .ToListAsync();

    public async Task<EnvironmentToolingItem?> GetByIdAsync(int id) =>
        await context.EnvironmentToolingItems
            .Include(e => e.Owner)
            .Include(e => e.EnvironmentToolingDefinition)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task AddAsync(EnvironmentToolingItem item)
    {
        await context.EnvironmentToolingItems.AddAsync(item);
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    public async Task<bool> EngagementExistsAsync(int engagementId) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);

    public async Task<bool> UserExistsAsync(int userId) =>
        await context.Users.AnyAsync(u => u.Id == userId);

    public async Task<bool> IsEngagementSignedOffAsync(int engagementId) =>
        await context.DayOneSignOffRecords.AsNoTracking().AnyAsync(r => r.EngagementId == engagementId);
}
