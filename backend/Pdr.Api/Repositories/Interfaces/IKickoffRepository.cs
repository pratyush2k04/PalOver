using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Kickoff;

namespace Pdr.Api.Repositories.Interfaces;

public interface IKickoffRepository
{
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<bool> UserHasAccessToEngagementAsync(int engagementId, int userId);
    Task<KickoffRecord?> GetByEngagementIdAsync(int engagementId);
    Task AddAsync(KickoffRecord record);
    void Update(KickoffRecord record);
    Task<IEnumerable<KickoffActionItem>> GetOpenActionItemsByEngagementIdAsync(int engagementId);
    Task<KickoffActionItem?> GetActionItemByIdAsync(int actionItemId);
    Task<Dictionary<int, KickoffActionItem>> GetActionItemsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task AddActionItemAsync(KickoffActionItem item);
    void UpdateActionItem(KickoffActionItem item);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> SaveChangesAsync();
}
