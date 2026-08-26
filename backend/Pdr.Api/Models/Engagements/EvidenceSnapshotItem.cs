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

    public string? EvidenceNote { get; set; }

    public string? EvidenceLink { get; set; }

    public DateTime? EvidenceSubmittedAt { get; set; }

    public string? EvidenceSubmittedByName { get; set; }

    [JsonIgnore]
    public EvidenceSnapshot EvidenceSnapshot { get; set; } = null!;
}
