using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverQueryQuestionRepository
{
    Task<HandoverQueryQuestion?> GetByQueryIdAsync(int queryId);
    Task<Dictionary<int, HandoverQueryQuestion>> GetByQueryIdsAsync(List<int> queryIds);
    Task AddAsync(HandoverQueryQuestion question);
    Task SaveChangesAsync();
}
