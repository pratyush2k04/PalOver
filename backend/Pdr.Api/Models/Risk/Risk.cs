using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Models.Risk
{
    public class Risk : BaseAuditableEntity
    {
        public int RiskId { get; set; }
        public int EngagementId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Engagement Engagement { get; set; } = null!;

        public int? AccessItemId { get; set; }
        public AccessItem? AccessItem { get; set; }

        public int? EnvironmentToolingItemId { get; set; }
        public EnvironmentToolingItem? EnvironmentToolingItem { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RiskStatus Status { get; set; } = RiskStatus.Open;
    }
}
