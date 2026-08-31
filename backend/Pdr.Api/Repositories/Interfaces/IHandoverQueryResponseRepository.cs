using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverQueryResponseRepository
{
    Task<HandoverQueryResponse?> GetByQueryIdAsync(int queryId);
    Task<Dictionary<int, HandoverQueryResponse>> GetByQueryIdsAsync(List<int> queryIds);
    Task AddAsync(HandoverQueryResponse response);
    Task SaveChangesAsync();
}
