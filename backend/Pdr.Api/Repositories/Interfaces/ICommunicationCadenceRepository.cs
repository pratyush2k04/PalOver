using Pdr.Api.Models.Cadence;

namespace Pdr.Api.Repositories.Interfaces;

public interface ICommunicationCadenceRepository
{
    Task<List<CommunicationCadence>> GetByEngagementIdAsync(int engagementId);
    Task<CommunicationCadence> AddAsync(CommunicationCadence cadence);
    Task<bool> ExistsByRoleAsync(int engagementId, string stakeholderRole);
}
