using Pdr.Api.Models.Handover;

namespace Pdr.Api.DTOs.Handovers;

public class CommitmentDto
{
    public int CommitmentId { get; set; }
    public int HandoverId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CommitmentTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Timeline { get; set; }
    public CommitmentTimelineUnitEnum TimelineUnit { get; set; }
    public RealisticCommitmentDto? RealisticCommitment { get; set; }
    public bool IsVarianceFlagged { get; set; }
    public string? VarianceDescription { get; set; }
}
