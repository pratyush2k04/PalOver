using Pdr.Api.Models.LoginEntity;
using Pdr.Api.DTOs.Home;

namespace Pdr.Api.Repositories.Interfaces;

public interface IHomeRepository
{
    Task<User?> GetUserWithRoleAsync(int userId);
    Task<List<int>> GetUserEngagementIdsAsync(int userId);
    Task<int> GetDraftHandoversCountAsync(List<int> engagementIds);
    Task<int> GetCompleteHandoversCountAsync(List<int> engagementIds);
    Task<int> GetIncompleteChecklistCountAsync(List<int> engagementIds);
    Task<int> GetOpenBlockersCountAsync(List<int> engagementIds);
    Task<int> GetOpenAccessItemsCountAsync(List<int> engagementIds);
    Task<int> GetMyEscalationsCountAsync(int userId);
    Task<List<DashboardEngagementDto>> GetUpcomingKickoffsAsync(List<int> engagementIds, DateOnly cutoffDate);
    Task<int> GetEngagementOpenBlockerCountAsync(int engagementId);
}
