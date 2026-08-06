namespace Pdr.Api.Models.Cadence;

public class CommunicationCadence : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public string StakeholderRole { get; set; } = string.Empty;

    public CadenceTypeEnum Cadence { get; set; }

    public string? Participants { get; set; }
}
