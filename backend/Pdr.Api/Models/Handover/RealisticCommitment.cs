namespace Pdr.Api.Models.Handover;

public class RealisticCommitment : BaseAuditableEntity
{
    public int CommitmentId { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Timeline { get; set; }

    public CommitmentTimelineUnitEnum TimelineUnit { get; set; }

    public string? AcknowledgementMessage { get; set; }

    public bool IsPossible { get; set; } = true;
}
