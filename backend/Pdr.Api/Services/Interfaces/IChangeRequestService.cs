using Pdr.Api.DTOs.ChangeRequests;

namespace Pdr.Api.Services.Interfaces;

public interface IChangeRequestService
{
    Task<List<ChangeRequestDto>> GetChangeRequestsAsync(int engagementId, string? status = null);
    Task<ChangeRequestDto> CreateChangeRequestAsync(int engagementId, CreateChangeRequestDto dto);
    Task<ChangeRequestDto> ApproveChangeRequestAsync(int id);
    Task<ChangeRequestDto> RejectChangeRequestAsync(int id, RejectChangeRequestDto dto);
}
