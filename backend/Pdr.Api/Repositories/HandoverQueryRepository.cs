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
            .Include(q => q.RaisedByUser)
            .Include(q => q.RespondedByUser)
            .Include(q => q.ResolvedByUser).ThenInclude(u => u!.Role)
            .Where(q => q.HandoverId == handoverId)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync();

    public async Task<HandoverQuery?> GetByIdAsync(int queryId, int handoverId) =>
        await context.HandoverQueries
            .Include(q => q.FieldSection)
            .Include(q => q.ResolvedByUser).ThenInclude(u => u!.Role)
            .Include(q => q.Response)
            .FirstOrDefaultAsync(q => q.Id == queryId && q.HandoverId == handoverId);

    public async Task AddAsync(HandoverQuery query) =>
        await context.HandoverQueries.AddAsync(query);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
