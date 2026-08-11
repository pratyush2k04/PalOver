namespace Pdr.Api.Models.Handover;

public class HandoverInfo : BaseAuditableEntity
{
    public int HandoverId { get; set; }

    public int EngagementId { get; set; }

    public int StatusId { get; set; }

    public string? ClientBackground { get; set; }

    public string BusinessProblem { get; set; } = string.Empty;

    public string ClientSentiment { get; set; } = string.Empty;

    public string? DealOrigin { get; set; }
}
