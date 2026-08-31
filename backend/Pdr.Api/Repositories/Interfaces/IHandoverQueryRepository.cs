using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverQueryRepository
{
    Task<List<HandoverQuery>> GetByHandoverIdAsync(int handoverId);
    Task<HandoverQuery?> GetByIdAsync(int queryId, int handoverId);
    Task AddAsync(HandoverQuery query);
    Task SaveChangesAsync();
}
