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
    Task<Dictionary<int, Engagement>> GetByIdsAsync(IEnumerable<int> engagementIds);
    Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync();
    Task<IEnumerable<CommercialModelDto>> GetCommercialModelsAsync();
    Task<bool> EngagementTypeExistsAsync(int id);
    Task<bool> CommercialModelExistsAsync(int id);
    Task<bool> EngagementExistsAsync(int id);
    Task<Engagement?> GetMatchingEngagementAsync(string name, string client, int typeId);
    Task<int> GetStatusIdByNameAsync(string statusName);
    Task AddEngagementAsync(Engagement engagement);
    Task<EngagementInfoDto?> GetEngagementInfoAsync(int id);
    Task<IEnumerable<EngagementInfoDto>> GetAllEngagementInfosAsync();
    Task AssignTechLeadOwnerItemsAsync(int engagementId, int userId);
    Task InitializeTechnicalReadinessItemsAsync(int engagementId, int ownerId);
    Task<EvidenceSnapshotDto> GetEvidenceSnapshotAsync(int engagementId);
    Task<bool> IsUserAssignedAsync(int engagementId, int userId);
    Task<List<EngagementReportInfoDto>> GetEngagementReportInfoAsync(List<int> engagementIds);
    Task<List<EngagementAssignment>> GetAssignmentsForEngagementsAsync(List<int> engagementIds);
}