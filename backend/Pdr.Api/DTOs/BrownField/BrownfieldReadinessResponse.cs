namespace Pdr.Api.DTOs.BrownField;

public class BrownfieldReadinessResponse
{
    public int EngagementId { get; set; }
    public int TotalCount { get; set; }
    public int HighSeverityPendingCount { get; set; }
    public bool IsTechnicallyReady { get; set; }
    public List<BrownfieldFindingDto> Items { get; set; } = new();
}
