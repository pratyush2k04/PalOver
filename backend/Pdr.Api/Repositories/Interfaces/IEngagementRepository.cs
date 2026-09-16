using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEngagementRepository
{
    Task<List<Engagement>> GetEngagementsForUserAsync(int userId, string sortBy);
    Task<(List<Engagement> Items, int TotalCount)> GetEngagementsForUserPageAsync(int userId, bool isSignedOff, string sortBy, int page, int pageSize);
    Task<List<Engagement>> GetEngagementsForUserFilteredAsync(int userId, bool isSignedOff, string sortBy);
    Task<int> GetEngagementsForUserStatusCountAsync(int userId, bool isSignedOff);
    Task<List<int>> GetEngagementIdsForUserAsync(int userId);
    Task<Engagement?> GetByIdAsync(int engagementId);
    Task SaveChangesAsync();
    //assignment details
    Task<List<EngagementAssignment>> GetAssignmentsAsync(int engagementId);
    Task<List<EngagementAssignment>> GetLeadershipAssignmentsAsync(int engagementId);
    Task<EngagementAssignment?> GetAssignmentByRoleIdAsync(int engagementId, int roleId);
    Task<EngagementAssignment?> GetAssignmentByUserAsync(int engagementId, int userId);
    Task RemoveAssignmentAsync(EngagementAssignment assignment);
    Task AddAssignmentAsync(int engagementId, int userId, int engagementRoleId = (int)EngagementRoleEnum.PreSales);
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
    Task<bool> IsEngagementSignedOffAsync(int engagementId);
    Task EnsureNotSignedOffAsync(int engagementId);
    Task<bool> HasAnyAssignmentAsync(int engagementId);
    Task<Engagement?> GetForOnePagerAsync(int engagementId);
}