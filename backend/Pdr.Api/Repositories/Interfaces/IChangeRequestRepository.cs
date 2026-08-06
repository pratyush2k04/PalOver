using Pdr.Api.Models.ChangeRequest;

namespace Pdr.Api.Repositories.Interfaces;

public interface IChangeRequestRepository
{
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<ChangeRequest?> GetByIdAsync(int id);
    Task<List<ChangeRequest>> GetByEngagementIdAsync(int engagementId, ChangeRequestStatus? statusFilter = null);
    Task<ChangeRequest> AddAsync(ChangeRequest changeRequest);
    Task SaveChangesAsync();
}
