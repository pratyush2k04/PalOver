using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementService
{
    Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request);
    Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync();
    Task<IEnumerable<CommercialModelDto>> GetCommercialModelsAsync();
    Task<EngagementInfoDto?> GetEngagementInfoAsync(int id);
    Task<IEnumerable<EngagementInfoDto>> GetAllEngagementsAsync();
    Task<bool> EditEngagementAsync(int id, EditEngagementRequest request);
    Task<EngagementListResponse> GetMyEngagementsAsync(string sortBy);
    Task<MyEngagementsPageDto> GetMyEngagementsPageAsync(string status, string sortBy, int page, int pageSize);
    //assigning
    Task<EngagementOwnersDto> GetEngagementOwnersAsync(int engagementId);

    Task<EngagementOwnersDto> AssignProjectManagerAsync(int engagementId, int userId);
    Task<EngagementOwnersDto> AssignTechLeadAsync(int engagementId, int userId);
    Task<IEnumerable<Pdr.Api.DTOs.Users.UserListItemDto>> GetEngagementUsersAsync(int engagementId);
    Task<List<LeadershipUserDto>> GetLeadershipUsersAsync(int engagementId);
    Task<List<LeadershipUserDto>> AssignLeadershipUserAsync(int engagementId, int userId);
    Task<List<LeadershipUserDto>> AssignLeadershipUsersAsync(int engagementId, List<int> userIds);
    Task<List<LeadershipUserDto>> RemoveLeadershipUserAsync(int engagementId, int userId);
    Task<List<LeadershipUserDto>> UpdateLeadershipUsersAsync(int engagementId, List<int> userIds);
    Task<HandoverDraftStatusDto> GetHandoverDraftStatusAsync(int engagementId);
    Task<List<LeadershipEngagementListItemDto>> GetLeadershipDashboardAsync();
    Task<EngagementOpenItemsSummaryDto?> GetOpenItemsSummaryAsync(int engagementId);
    Task<DayOneSignOffDto> SignOffDayOneAsync(int engagementId, DayOneSignOffRequest request);
    Task<EvidenceSnapshotDto> GetEvidenceSnapshotAsync(int engagementId);
}
