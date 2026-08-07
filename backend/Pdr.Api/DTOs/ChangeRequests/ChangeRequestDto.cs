namespace Pdr.Api.DTOs.ChangeRequests;

public class ChangeRequestDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Under Review", "Approved", "Rejected"
    public string? RejectionReason { get; set; }
}
