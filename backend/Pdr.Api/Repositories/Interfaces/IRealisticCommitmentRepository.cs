using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRealisticCommitmentRepository
{
    Task<Dictionary<int, RealisticCommitment>> GetByCommitmentIdsAsync(List<int> commitmentIds);
    Task<RealisticCommitment?> GetByCommitmentIdAsync(int commitmentId);
    Task AddAsync(RealisticCommitment realisticCommitment);
    Task SaveChangesAsync();
}
