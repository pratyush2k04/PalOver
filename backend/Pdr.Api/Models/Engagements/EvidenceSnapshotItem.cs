using System;
using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Engagements;

public class EvidenceSnapshotItem : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EvidenceSnapshotId { get; set; }

    public int? SourceChecklistItemId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public EvidenceSnapshotItemStatus StatusAtSignOff { get; set; }

    public int? EvidenceDocumentId { get; set; }

    public string? EvidenceDocumentFileName { get; set; }

    public DateTime? EvidenceSubmittedAt { get; set; }

    public string? EvidenceSubmittedByName { get; set; }

    public int? OwnerId { get; set; }

    public string? OwnerName { get; set; }

    [JsonIgnore]
    public EvidenceSnapshot EvidenceSnapshot { get; set; } = null!;
}
