using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class CommitmentRepository(AppDbContext context) : ICommitmentRepository
{
    public async Task<List<Commitment>> GetByHandoverIdAsync(int handoverId) =>
        await context.Commitments
            .AsNoTracking()
            .Where(c => c.HandoverId == handoverId)
            .ToListAsync();

    public async Task<int> GetCountByHandoverIdAsync(int handoverId) =>
        await context.Commitments
            .Where(c => c.HandoverId == handoverId)
            .CountAsync();

    public async Task<Commitment?> GetByIdAsync(int commitmentId, int handoverId) =>
        await context.Commitments
            .FirstOrDefaultAsync(c => c.CommitmentId == commitmentId && c.HandoverId == handoverId);

    public async Task AddAsync(Commitment commitment) =>
        await context.Commitments.AddAsync(commitment);

    public Task RemoveAsync(Commitment commitment)
    {
        context.Commitments.Remove(commitment);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
