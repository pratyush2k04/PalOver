namespace Pdr.Api.DTOs.Assumptions;

public class UpdateAssumptionDto
{
    public string Description { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
