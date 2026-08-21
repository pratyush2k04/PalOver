using Pdr.Api.DTOs.Dependencies;

namespace Pdr.Api.Services.Interfaces;

public interface IDependencyService
{
    Task<IEnumerable<DependencyDto>> GetDependenciesAsync(int engagementId);
    Task<DependencyDto> CreateDependencyAsync(int engagementId, CreateDependencyDto dto);
    Task<DependencyDto> UpdateDependencyAsync(int engagementId, int id, UpdateDependencyDto dto);
    Task DeleteDependencyAsync(int engagementId, int id);
}
