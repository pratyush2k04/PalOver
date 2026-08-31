using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Home;

namespace Pdr.Api.Services.Interfaces;

public interface IHomeService
{
    Task<HomeResponseDto> GetHomeAsync(int userId);
    Task<PagedResult<DashboardEngagementDto>> GetDashboardEngagementsPageAsync(int userId, int page, int pageSize);
}
