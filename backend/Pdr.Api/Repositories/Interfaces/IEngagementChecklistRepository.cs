using Pdr.Api.Models.EngagementChecklist;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEngagementChecklistRepository
{
    Task<EngagementChecklist?> GetByEngagementIdAsync(int engagementId);
    Task AddAsync(EngagementChecklist checklist);
    Task SaveChangesAsync();
}
