using Pdr.Api.Models.Handover;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverQueryMessageRepository
{
    Task<List<HandoverQueryMessage>> GetByQueryIdAsync(int queryId);
    Task<HandoverQueryMessage?> GetLatestMessageByQueryIdAsync(int queryId);
    Task<int> GetNextSequenceNumberAsync(int queryId);
    Task AddAsync(HandoverQueryMessage message);
    Task SaveChangesAsync();
}
