using Pdr.Api.Models;

namespace Pdr.Api.Repositories.Interfaces;

public interface IKickoffRepository
{
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<bool> UserHasAccessToEngagementAsync(int engagementId, int userId);
    Task<KickoffRecord?> GetByEngagementIdAsync(int engagementId);
    Task AddAsync(KickoffRecord record);
    void Update(KickoffRecord record);
    Task<bool> SaveChangesAsync();
}
