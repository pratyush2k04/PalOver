using Pdr.Api.DTOs.DependencySpikes;

namespace Pdr.Api.Services.Interfaces;

public interface IDependencySpikeService
{
    Task<List<DependencySpikeDto>> GetSpikesByEngagementIdAsync(int engagementId);
    Task<DependencySpikeDto> CreateSpikeAsync(int engagementId, CreateDependencySpikeRequest request);
    Task<DependencySpikeDto> UpdateSpikeAsync(int spikeId, UpdateDependencySpikeRequest request);
    Task DeleteSpikeAsync(int spikeId);
}
