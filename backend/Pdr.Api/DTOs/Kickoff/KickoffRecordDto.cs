namespace Pdr.Api.DTOs.Kickoff;

public class KickoffRecordDto
{
    public int KickoffRecordId { get; set; }
    public int EngagementId { get; set; }
    public string Outcomes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
