using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.EngagementAssignment;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEngagementRepository
{
    Task<List<Engagement>> GetEngagementsForUserAsync(int userId, string sortBy);
    Task<List<int>> GetEngagementIdsForUserAsync(int userId);
    Task<Engagement?> GetByIdAsync(int engagementId);
    Task SaveChangesAsync();
    //assignment details
    Task<List<EngagementAssignment>> GetAssignmentsAsync(int engagementId);
    Task<List<EngagementAssignment>> GetLeadershipAssignmentsAsync(int engagementId);
    Task<EngagementAssignment?> GetAssignmentByRoleIdAsync(int engagementId, int roleId);
    Task<EngagementAssignment?> GetAssignmentByUserAsync(int engagementId, int userId);
    Task RemoveAssignmentAsync(EngagementAssignment assignment);
    Task AddAssignmentAsync(int engagementId, int userId);
    Task<EngagementOpenItemsSummaryDto> GetOpenItemsSummaryAsync(int engagementId);
}