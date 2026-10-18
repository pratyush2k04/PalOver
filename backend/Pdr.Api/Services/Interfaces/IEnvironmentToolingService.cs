using Pdr.Api.DTOs.EnvironmentTooling;

namespace Pdr.Api.Services.Interfaces;

public interface IEnvironmentToolingService
{
    Task<EnvironmentToolingReadinessResponse> GetItemsByEngagementIdAsync(int engagementId);
    Task<EnvironmentToolingItemDto> AddCustomItemAsync(int engagementId, AddCustomEnvironmentToolingItemRequest request);
    Task<EnvironmentToolingItemDto> UpdateStatusAsync(int id, UpdateEnvironmentToolingStatusRequest request);
    Task<EnvironmentToolingItemDto> UpdateOwnerAsync(int id, UpdateEnvironmentToolingOwnerRequest request);
    Task DeleteItemAsync(int id);
}
