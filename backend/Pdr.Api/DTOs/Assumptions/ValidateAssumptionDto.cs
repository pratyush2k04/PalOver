using Pdr.Api.Models.Engagements;

namespace Pdr.Api.DTOs.Assumptions;

public class ValidateAssumptionDto
{
    public AssumptionStatus Status { get; set; }
    
    // Set to true if the client wants to automatically create a risk when Status == Invalid
    public bool CreateRisk { get; set; } = false;
}
