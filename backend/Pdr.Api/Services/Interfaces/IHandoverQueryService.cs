using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverQueryService
{
    Task<List<HandoverQueryDto>> GetQueriesAsync(int engagementId);
    Task<HandoverQueryDto> RaiseQueryAsync(int engagementId, CreateHandoverQueryRequest request, string userIdStr);
    Task<HandoverQueryDto> RespondToQueryAsync(int engagementId, int queryId, RespondHandoverQueryRequest request, string userIdStr);
    Task<HandoverQueryDto> ResolveQueryAsync(int engagementId, int queryId, ResolveHandoverQueryRequest request, string userIdStr);
}
