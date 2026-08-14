using Pdr.Api.DTOs.ArchitectureValidation;

namespace Pdr.Api.Services.Interfaces;

public interface IArchitectureValidationService
{
    Task<ArchitectureValidationDto> GetByEngagementIdAsync(int engagementId);
    Task<ArchitectureValidationDto> SaveValidationAsync(int engagementId, SaveArchitectureValidationRequest request);
    Task<DiscoveryFeatureItemDto> AddFeatureItemAsync(int engagementId, CreateDiscoveryFeatureItemRequest request);
    Task<DiscoveryFeatureItemDto> UpdateFeatureItemAsync(int featureId, UpdateDiscoveryFeatureItemRequest request);
    Task DeleteFeatureItemAsync(int featureId);
}
