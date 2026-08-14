using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HandoverQueryRepository(AppDbContext context) : IHandoverQueryRepository
{
    public async Task<List<HandoverQuery>> GetByHandoverIdAsync(int handoverId) =>
        await context.HandoverQueries
            .AsNoTracking()
            .Include(q => q.FieldSection)
            .Include(q => q.ResolvedByUser).ThenInclude(u => u!.Role)
            .Where(q => q.HandoverId == handoverId)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync();

    public async Task<HandoverQuery?> GetByIdAsync(int queryId, int handoverId) =>
        await context.HandoverQueries
            .Include(q => q.FieldSection)
            .Include(q => q.ResolvedByUser).ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(q => q.QueryId == queryId && q.HandoverId == handoverId);

    public async Task AddAsync(HandoverQuery query) =>
        await context.HandoverQueries.AddAsync(query);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
