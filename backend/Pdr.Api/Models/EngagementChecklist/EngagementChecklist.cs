using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.EngagementChecklist;

public class EngagementChecklist : BaseAuditableEntity
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public int SourceMasterVersionId { get; set; }
    public int VersionNumber { get; set; }

    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public Engagement Engagement { get; set; } = null!;
    public ICollection<EngagementChecklistItem> Items { get; set; } = new List<EngagementChecklistItem>();
}
