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
    
    // Evidence tracking
    public int? EvidenceDocumentId { get; set; }
    public DateTime? EvidenceSubmittedAt { get; set; }
    public int? EvidenceSubmittedById { get; set; }
    
    public Engagement Engagement { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public HandoverReadinessDefinition? HandoverReadinessDefinition { get; set; }
    public Documents.Document? EvidenceDocument { get; set; }
    public User? EvidenceSubmittedBy { get; set; }
}
