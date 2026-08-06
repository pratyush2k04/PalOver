using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEngagementRepository
{
    Task<List<Engagement>> GetEngagementsForPmAsync(int userId, string sortBy);
    //assignment details 
    Task<List<EngagementAssignment>> GetAssignmentsAsync(int engagementId);
    Task<EngagementAssignment?> GetAssignmentByRoleIdAsync(int engagementId, int roleId);
    Task RemoveAssignmentAsync(EngagementAssignment assignment);
    Task AddAssignmentAsync(int engagementId, int userId);
}
