namespace Pdr.Api.Models.Handover;

public class Commitment : BaseAuditableEntity
{
    public int CommitmentId { get; set; }

    public int HandoverId { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CommitmentTypeId { get; set; }

    public int Timeline { get; set; }

    public CommitmentTimelineUnitEnum TimelineUnit { get; set; }
}
