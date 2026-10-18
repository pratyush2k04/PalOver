using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HandoverQueryMessageRepository(AppDbContext context) : IHandoverQueryMessageRepository
{
    public async Task<List<HandoverQueryMessage>> GetByQueryIdAsync(int queryId) =>
        await context.HandoverQueryMessages
            .AsNoTracking()
            .Include(m => m.SenderUser).ThenInclude(u => u.Role)
            .Where(m => m.QueryId == queryId)
            .OrderBy(m => m.SequenceNumber)
            .ToListAsync();

    public async Task<HandoverQueryMessage?> GetLatestMessageByQueryIdAsync(int queryId) =>
        await context.HandoverQueryMessages
            .AsNoTracking()
            .Include(m => m.SenderUser).ThenInclude(u => u.Role)
            .Where(m => m.QueryId == queryId)
            .OrderByDescending(m => m.SequenceNumber)
            .FirstOrDefaultAsync();

    public async Task<int> GetNextSequenceNumberAsync(int queryId)
    {
        var maxSeq = await context.HandoverQueryMessages
            .Where(m => m.QueryId == queryId)
            .MaxAsync(m => (int?)m.SequenceNumber);

        return (maxSeq ?? 0) + 1;
    }

    public async Task AddAsync(HandoverQueryMessage message) =>
        await context.HandoverQueryMessages.AddAsync(message);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
