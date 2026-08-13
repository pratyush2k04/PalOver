using System.Text.Json.Serialization;

namespace Pdr.Api.Models.ArchitectureValidation;

public class DiscoveryFeatureItem : BaseAuditableEntity
{
    public int DiscoveryFeatureItemId { get; set; }

    public int ArchitectureValidationId { get; set; }

    [JsonIgnore]
    public ArchitectureValidation ArchitectureValidation { get; set; } = null!;

    public string FeatureName { get; set; } = string.Empty;

    public decimal EstimateWeeks { get; set; }

    public string TargetSprint { get; set; } = string.Empty;
}
