using Pdr.Api.Models;

namespace Pdr.Api.Repositories.Interfaces;

public interface IClientContactTransitionRepository
{
    Task<ClientContactTransition?> GetByEngagementIdAsync(int engagementId);
    Task<List<ClientContactTransition>> GetAllByEngagementIdAsync(int engagementId);
    Task AddAsync(ClientContactTransition transition);
    void Update(ClientContactTransition transition);
    Task<bool> SaveChangesAsync();
}
