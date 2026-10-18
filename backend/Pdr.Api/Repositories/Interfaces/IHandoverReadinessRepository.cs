using System.Collections.Generic;
using System.Threading.Tasks;
using Pdr.Api.Models.HandoverReadiness;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHandoverReadinessRepository
{
    Task<List<HandoverReadinessItem>> GetItemsByEngagementIdAsync(int engagementId);
    Task<HandoverReadinessItem?> GetByIdAsync(int id);
    Task<List<HandoverReadinessDefinition>> GetActiveDefinitionsAsync();
    Task AddItemAsync(HandoverReadinessItem item);
    Task AddItemsAsync(IEnumerable<HandoverReadinessItem> items);
    Task<bool> EngagementExistsAsync(int engagementId);
    Task SaveChangesAsync();
}
