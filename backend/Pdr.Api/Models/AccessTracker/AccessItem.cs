using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.AccessTracker
{
    public class AccessItem : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EngagementId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Engagement Engagement { get; set; } = null!;
        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public AccessItemStatus Status { get; set; } = AccessItemStatus.InProgress;
        public int? AccessItemDefinitionId { get; set; }
        public AccessItemDefinition? AccessItemDefinition { get; set; }
        public bool IsClientSide { get; set; }
        public string? TimeZoneNote { get; set; }
        public int? ExpectedTurnaroundDays { get; set; }
        public DateOnly? GrantedDate { get; set; }
        public string? BlockReason { get; set; }
    }
}
