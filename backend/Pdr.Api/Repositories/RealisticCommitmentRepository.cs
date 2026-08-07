using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class RealisticCommitmentRepository(AppDbContext context) : IRealisticCommitmentRepository
{
    public async Task<Dictionary<int, RealisticCommitment>> GetByCommitmentIdsAsync(List<int> commitmentIds) =>
        await context.RealisticCommitments
            .AsNoTracking()
            .Where(rc => commitmentIds.Contains(rc.CommitmentId))
            .ToDictionaryAsync(rc => rc.CommitmentId);

    public async Task<RealisticCommitment?> GetByCommitmentIdAsync(int commitmentId) =>
        await context.RealisticCommitments
            .FirstOrDefaultAsync(rc => rc.CommitmentId == commitmentId);

    public async Task AddAsync(RealisticCommitment realisticCommitment) =>
        await context.RealisticCommitments.AddAsync(realisticCommitment);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
