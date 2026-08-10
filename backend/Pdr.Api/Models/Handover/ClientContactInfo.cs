namespace Pdr.Api.Models.Handover;

public class ClientContactInfo : BaseAuditableEntity
{
    public int ClientContactInfoId { get; set; }

    public int EngagementId { get; set; }

    public Engagement Engagement { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
