using Pdr.Api.Models.ArchitectureValidation;

namespace Pdr.Api.DTOs.ArchitectureValidation;

public class SaveArchitectureValidationRequest
{
    public ArchitectureStatus Status { get; set; }
    public string? Rationale { get; set; }
}
