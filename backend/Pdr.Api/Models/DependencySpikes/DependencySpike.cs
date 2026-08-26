using System.Text.Json.Serialization;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.DependencySpikes;

public class DependencySpike : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    [JsonIgnore]
    public Engagement Engagement { get; set; } = null!;

    public string DependencyName { get; set; } = string.Empty;

    public string Objective { get; set; } = string.Empty;

    public DependencySpikeStatus Status { get; set; } = DependencySpikeStatus.InProgress;

    public int? OwnerId { get; set; }

    [JsonIgnore]
    public User? Owner { get; set; }

    public string? OutcomeFindings { get; set; }
}
