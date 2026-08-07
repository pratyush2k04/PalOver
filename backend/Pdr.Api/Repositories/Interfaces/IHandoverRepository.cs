using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverRepository
{
    Task<HandoverInfo?> GetByEngagementIdAsync(int engagementId);
    Task AddAsync(HandoverInfo handover);
    Task<HandoverStatus?> GetStatusByIdAsync(int statusId);
    Task SaveChangesAsync();
}
