using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.HandoverReadiness;

public class HandoverReadinessItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string Area { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public HandoverReadinessStatus Status { get; set; } = HandoverReadinessStatus.InProgress;
    public string? BlockReason { get; set; }
    public int? HandoverReadinessDefinitionId { get; set; }
    
    public Engagement Engagement { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public HandoverReadinessDefinition? HandoverReadinessDefinition { get; set; }
}
