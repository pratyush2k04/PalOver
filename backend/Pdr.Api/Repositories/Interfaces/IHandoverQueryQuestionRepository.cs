using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverQueryQuestionRepository
{
    Task<HandoverQueryQuestion?> GetByQueryIdAsync(int queryId);
    Task AddAsync(HandoverQueryQuestion question);
    Task SaveChangesAsync();
}
