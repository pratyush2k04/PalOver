using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverQueriesService
{
    Task<List<HandoverQueryDto>> GetQueriesAsync(int engagementId);
    Task<HandoverQueryDto> RaiseQueryAsync(int engagementId, CreateHandoverQueryRequest request, string userIdStr);
    Task<HandoverQueryDto> RespondToQueryAsync(int engagementId, int queryId, RespondHandoverQueryRequest request, string userIdStr);
    Task<HandoverQueryDto> ToggleResolutionAsync(int engagementId, int queryId, string userIdStr);
}
