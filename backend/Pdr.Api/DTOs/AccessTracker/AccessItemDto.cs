namespace Pdr.Api.DTOs.AccessTracker;

public class AccessItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsClientSide { get; set; }
    public int OwnerId { get; set; }
    public string OwnerFullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TimeZoneNote { get; set; }
    public int? ExpectedTurnaroundDays { get; set; }
    public int? DaysUntilBillingDate { get; set; }
    public bool IsOverdueWithClient { get; set; }
    public DateOnly? GrantedDate { get; set; }
    public string? BlockReason { get; set; }
}
