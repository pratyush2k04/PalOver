namespace Pdr.Api.DTOs.ArchitectureValidation;

public class UpdateDiscoveryFeatureItemRequest
{
    public required string FeatureName { get; set; }
    public decimal EstimateWeeks { get; set; }
    public required string TargetSprint { get; set; }
}
