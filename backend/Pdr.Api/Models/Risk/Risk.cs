namespace Pdr.Api.Models.Risk
{
    public class Risk : BaseAuditableEntity
    {
        public int RiskId { get; set; }
        public int EngagementId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Engagement Engagement { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RiskStatus Status { get; set; } = RiskStatus.Open;
    }
}
