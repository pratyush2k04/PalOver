using Pdr.Api.Models.BrownField;

namespace Pdr.Api.Repositories.Interfaces;

public interface IBrownfieldFindingRepository
{
    Task<List<BrownfieldFinding>> GetByEngagementIdAsync(int engagementId);
    Task<BrownfieldFinding?> GetByIdAsync(int findingId);
    Task AddAsync(BrownfieldFinding finding);
    Task SaveChangesAsync();
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<bool> DocumentExistsAsync(int documentId);
    Task<bool> IsEngagementSignedOffAsync(int engagementId);
}
