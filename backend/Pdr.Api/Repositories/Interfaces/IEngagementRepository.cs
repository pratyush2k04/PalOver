using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEngagementRepository
{
    Task<List<Engagement>> GetEngagementsForPmAsync(int userId, string sortBy);
    //assignment details 
    Task<List<EngagementAssignment>> GetAssignmentsAsync(long engagementId);
    Task<EngagementAssignment?> GetAssignmentByRoleAsync(long engagementId, string roleName);
    Task RemoveAssignmentAsync(EngagementAssignment assignment);
    Task AddAssignmentAsync(long engagementId, int userId);
}
