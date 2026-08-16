using System.Text.Json.Serialization;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.ArchitectureValidation;

public class ArchitectureValidation : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    [JsonIgnore]
    public Engagement Engagement { get; set; } = null!;

    public ArchitectureStatus Status { get; set; } = ArchitectureStatus.StillValid;

    public string? Rationale { get; set; }

    public ICollection<DiscoveryFeatureItem> Features { get; set; } = new List<DiscoveryFeatureItem>();
}
