using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Engagements;

public class EvidenceSnapshot : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public int SignedOffByUserId { get; set; }

    public string SignedOffByUserName { get; set; } = string.Empty;

    public DateTime SignedOffAt { get; set; }

    public DateOnly BillingStartDate { get; set; }

    [JsonIgnore]
    public Engagement Engagement { get; set; } = null!;

    public User SignedOffByUser { get; set; } = null!;

    public ICollection<EvidenceSnapshotItem> Items { get; set; } = new List<EvidenceSnapshotItem>();
}
