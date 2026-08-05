using Pdr.Api.DTOs.Kickoff;

namespace Pdr.Api.Services.Interfaces;

public interface IKickoffService
{
    Task<KickoffRecordDto> GetKickoffRecordAsync(int engagementId, string? currentUserId);
    Task<KickoffRecordDto> SaveKickoffRecordAsync(int engagementId, SaveKickoffRecordRequest request, string? currentUserId);
}
