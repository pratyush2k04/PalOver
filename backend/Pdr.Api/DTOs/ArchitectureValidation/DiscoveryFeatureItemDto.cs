namespace Pdr.Api.DTOs.ArchitectureValidation;

public class DiscoveryFeatureItemDto
{
    public int Id { get; set; }
    public int ArchitectureValidationId { get; set; }
    public string FeatureName { get; set; } = string.Empty;
    public decimal EstimateWeeks { get; set; }
    public string TargetSprint { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
