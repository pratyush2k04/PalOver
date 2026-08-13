using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models;

public class KickoffActionItem : BaseAuditableEntity
{
    public int KickoffActionItemId { get; set; }

    public int KickoffRecordId { get; set; }

    public KickoffRecord KickoffRecord { get; set; } = null!;

    public int EngagementId { get; set; }

    public Engagement Engagement { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public int? AssignedToUserId { get; set; }

    public User? AssignedToUser { get; set; }

    public DateOnly DueDate { get; set; }

    public KickoffActionItemStatus Status { get; set; } = KickoffActionItemStatus.Open;

    public bool IsNotified { get; set; } = true;
}
