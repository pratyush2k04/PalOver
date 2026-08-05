namespace Pdr.Api.DTOs.EnvironmentTooling;

public class EnvironmentToolingReadinessResponse
{
    public int EngagementId { get; set; }
    public int ReadyCount { get; set; }
    public int TotalCount { get; set; }
    public bool IsTechnicallyReady { get; set; }
    public string SummaryText => $"Ready {ReadyCount} of {TotalCount}";
    public List<EnvironmentToolingItemDto> OutstandingItems { get; set; } = new();
    public List<EnvironmentToolingItemDto> Items { get; set; } = new();
}
