using Pdr.Api.DTOs.Kickoff;
using Pdr.Api.Models;
using Pdr.Api.Models.Kickoff;

namespace Pdr.Api.Services.Interfaces;

public interface IKickoffService
{
    Task<KickoffRecordDto> GetKickoffRecordAsync(int engagementId, string? currentUserId);
    Task<KickoffRecordDto> SaveKickoffRecordAsync(int engagementId, SaveKickoffRecordRequest request, string? currentUserId);

    Task<IEnumerable<KickoffActionItemDto>> GetOpenActionItemsAsync(int engagementId, string? currentUserId);
    Task<KickoffActionItemDto> CreateActionItemAsync(int engagementId, KickoffActionItemDto dto, string? currentUserId);
    Task<KickoffActionItemDto> UpdateActionItemStatusAsync(int engagementId, int actionItemId, KickoffActionItemStatus status, string? currentUserId);
    Task<KickoffActionItemDto> UpdateActionItemAsync(int engagementId, int actionItemId, KickoffActionItemDto dto, string? currentUserId);
}
