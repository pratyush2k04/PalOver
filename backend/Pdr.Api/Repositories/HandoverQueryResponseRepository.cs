using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HandoverQueryResponseRepository(AppDbContext context) : IHandoverQueryResponseRepository
{
    public async Task<HandoverQueryResponse?> GetByQueryIdAsync(int queryId) =>
        await context.HandoverQueryResponses
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.QueryId == queryId);

    public async Task<Dictionary<int, HandoverQueryResponse>> GetByQueryIdsAsync(List<int> queryIds) =>
        await context.HandoverQueryResponses
            .AsNoTracking()
            .Where(r => queryIds.Contains(r.QueryId))
            .ToDictionaryAsync(r => r.QueryId);

    public async Task AddAsync(HandoverQueryResponse response) =>
        await context.HandoverQueryResponses.AddAsync(response);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
