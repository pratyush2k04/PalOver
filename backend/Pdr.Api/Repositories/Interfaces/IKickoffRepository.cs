using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;

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
    Task AddActionItemAsync(KickoffActionItem item);
    void UpdateActionItem(KickoffActionItem item);
    void DeleteActionItem(KickoffActionItem item);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> SaveChangesAsync();
}
