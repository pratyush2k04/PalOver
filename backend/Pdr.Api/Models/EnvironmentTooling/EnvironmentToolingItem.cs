using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.EnvironmentTooling 
{
    public class EnvironmentToolingItem : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? EnvironmentToolingDefinitionId { get; set; }
        public EnvironmentToolingDefinition? EnvironmentToolingDefinition { get; set; }

        public int EngagementId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Engagement Engagement { get; set; } = null!;

        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        // Used only for user-added custom items
        public string? CustomName { get; set; }

        public EnvironmentToolingStatus Status { get; set; }
            = EnvironmentToolingStatus.NotReady;

        // Required when Status = Blocked
        public string? BlockReason { get; set; }
    }
}
