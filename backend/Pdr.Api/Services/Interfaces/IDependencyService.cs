using Pdr.Api.DTOs.Dependencies;

namespace Pdr.Api.Services.Interfaces;

public interface IDependencyService
{
    Task<IEnumerable<DependencyDto>> GetDependenciesAsync(int engagementId, string? currentUserId);
    Task<DependencyDto> CreateDependencyAsync(int engagementId, CreateDependencyDto dto, string? currentUserId);
    Task<DependencyDto> UpdateDependencyAsync(int engagementId, int id, UpdateDependencyDto dto, string? currentUserId);
    Task DeleteDependencyAsync(int engagementId, int id, string? currentUserId);
    Task<DependencyDto> EscalateToRiskAsync(int engagementId, int id, string? currentUserId);
}
