using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.Handover;

public class ClientContactTransition : BaseAuditableEntity
{
    public int ClientContactTransitionId { get; set; }

    public int EngagementId { get; set; }

    public Engagement Engagement { get; set; } = null!;

    public string OutgoingContactName { get; set; } = string.Empty;

    public string OutgoingContactRole { get; set; } = string.Empty;

    public string IncomingContactName { get; set; } = string.Empty;

    public string IncomingContactRole { get; set; } = string.Empty;

    public string? IncomingContactEmail { get; set; }

    public string? IncomingContactBackground { get; set; }

    public string? IncomingContactSentiment { get; set; }
}
