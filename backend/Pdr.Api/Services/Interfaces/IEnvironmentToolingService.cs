using Pdr.Api.DTOs.EnvironmentTooling;

namespace Pdr.Api.Services.Interfaces;

public interface IEnvironmentToolingService
{
    Task<List<EnvironmentToolingItemDto>> GetItemsByEngagementIdAsync(int engagementId);
    Task<EnvironmentToolingItemDto> AddCustomItemAsync(int engagementId, AddCustomEnvironmentToolingItemRequest request);
    Task<EnvironmentToolingItemDto> UpdateStatusAsync(int id, UpdateEnvironmentToolingStatusRequest request);
    Task<EnvironmentToolingItemDto> UpdateOwnerAsync(int id, UpdateEnvironmentToolingOwnerRequest request);
}
