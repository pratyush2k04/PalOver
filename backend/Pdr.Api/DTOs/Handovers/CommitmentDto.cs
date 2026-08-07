namespace Pdr.Api.DTOs.Handovers;

public class CommitmentDto
{
    public int CommitmentId { get; set; }
    public int HandoverId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CommitmentTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Timeline { get; set; }
    public string TimelineUnit { get; set; } = string.Empty;
    public RealisticCommitmentDto? RealisticCommitment { get; set; }
}
