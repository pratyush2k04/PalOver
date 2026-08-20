using System;
using System.Text.Json.Serialization;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Engagements;

public class DayOneSignOffRecord : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    [JsonIgnore]
    public Engagement Engagement { get; set; } = null!;

    public int SignedOffByUserId { get; set; }

    public User SignedOffByUser { get; set; } = null!;

    public DateOnly BillingStartDate { get; set; }

    public DateTime SignedOffAt { get; set; }

    public bool IsException { get; set; }

    public string? ExceptionJustification { get; set; }
}
