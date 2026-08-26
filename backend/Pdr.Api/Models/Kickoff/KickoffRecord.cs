using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.Kickoff;

public class KickoffRecord : BaseAuditableEntity
{
    public int KickoffRecordId { get; set; }

    public int EngagementId { get; set; }

    public Engagement Engagement { get; set; } = null!;

    public string Outcomes { get; set; } = string.Empty;

    public ICollection<KickoffActionItem> ActionItems { get; set; } = new List<KickoffActionItem>();
}
