using Pdr.Api.Models.Kickoff;

namespace Pdr.Api.DTOs.Kickoff;

public class KickoffActionItemDto
{
    public int KickoffActionItemId { get; set; }
    public int KickoffRecordId { get; set; }
    public int EngagementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public int? AssignedToUserId { get; set; }
    public DateOnly DueDate { get; set; }
    public KickoffActionItemStatus Status { get; set; } = KickoffActionItemStatus.InProgress;
    public bool IsNotified { get; set; } = true;
}
