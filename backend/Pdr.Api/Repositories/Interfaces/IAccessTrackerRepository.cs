using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.DTOs.AccessTracker;

namespace Pdr.Api.Repositories.Interfaces;

public interface IAccessTrackerRepository
{
    // Access Item queries
    Task<List<AccessItem>> GetAccessItemsByEngagementIdAsync(int engagementId);
    Task<AccessItem?> GetAccessItemByIdAsync(int id);
    Task AddAccessItemAsync(AccessItem item);
    Task SaveChangesAsync();
    Task<List<AccessItemDelayInfoDto>> GetResolvedAccessItemsByEngagementIdsAsync(List<int> engagementIds);

    // Validation lookups (self-contained — no service touches DbContext directly)
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<bool> UserExistsAsync(int userId);
    Task<Engagement?> GetEngagementAsync(int engagementId);

    // Returns the UserId of the Tech Lead assigned to the engagement, or null if not yet assigned
    Task<int?> GetTechLeadUserIdAsync(int engagementId);
    Task<bool> IsEngagementSignedOffAsync(int engagementId);
}
