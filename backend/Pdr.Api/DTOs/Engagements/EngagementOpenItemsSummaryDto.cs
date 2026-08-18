namespace Pdr.Api.DTOs.Engagements;

public class EngagementOpenItemsSummaryDto
{
    public int EngagementId { get; set; }
    public int ChecklistOpenCount { get; set; }
    public int AccessOpenCount { get; set; }
    public int OpenRisksCount { get; set; }
    public int ResourcingOpenCount { get; set; }
}
