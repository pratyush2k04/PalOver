using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface ICommitmentRepository
{
    Task<List<Commitment>> GetByHandoverIdAsync(int handoverId);
    Task<int> GetCountByHandoverIdAsync(int handoverId);
    Task<Commitment?> GetByIdAsync(int commitmentId, int handoverId);
    Task AddAsync(Commitment commitment);
    Task RemoveAsync(Commitment commitment);
    Task SaveChangesAsync();
}
