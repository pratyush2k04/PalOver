using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverQueryResponseRepository
{
    Task<HandoverQueryResponse?> GetByQueryIdAsync(int queryId);
    Task AddAsync(HandoverQueryResponse response);
    Task SaveChangesAsync();
}
