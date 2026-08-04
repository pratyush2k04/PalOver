namespace Pdr.Api.DTOs.Handovers;

public class UpdateRealisticCommitmentRequest
{
    public string Description { get; set; } = string.Empty;
    public int Timeline { get; set; }
    public string TimelineUnit { get; set; } = string.Empty;
    public string? AcknowledgementMessage { get; set; }
    public bool IsPossible { get; set; } = true;
}
