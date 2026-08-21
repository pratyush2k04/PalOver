using Pdr.Api.DTOs.Assumptions;

namespace Pdr.Api.Services.Interfaces;

public interface IAssumptionService
{
    Task<List<AssumptionDto>> GetAssumptionsAsync(int engagementId);
    Task<AssumptionDto> GetAssumptionAsync(int engagementId, int assumptionId);
    Task<AssumptionDto> CreateAssumptionAsync(int engagementId, CreateAssumptionDto createDto);
    Task<AssumptionDto> UpdateAssumptionAsync(int engagementId, int assumptionId, UpdateAssumptionDto updateDto);
    Task<AssumptionDto> ValidateAssumptionAsync(int engagementId, int assumptionId, ValidateAssumptionDto validateDto);
}
