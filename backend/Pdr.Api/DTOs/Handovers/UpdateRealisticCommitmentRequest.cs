using Pdr.Api.Models.Handover;

namespace Pdr.Api.DTOs.Handovers;

public class UpdateRealisticCommitmentRequest
{
    public string Description { get; set; } = string.Empty;
    public int Timeline { get; set; }
    public CommitmentTimelineUnitEnum TimelineUnit { get; set; }
    public string? AcknowledgementMessage { get; set; }
}
