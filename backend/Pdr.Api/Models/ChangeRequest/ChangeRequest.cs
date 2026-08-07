namespace Pdr.Api.Models.ChangeRequest;

public class ChangeRequest : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Impact { get; set; } = string.Empty;

    public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UnderReview;

    public string? RejectionReason { get; set; }

    public int? ReviewedById { get; set; }

    public DateTime? ReviewedAt { get; set; }
}
