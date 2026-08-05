namespace Pdr.Api.Models;

public class Engagement : BaseAuditableEntity
{
    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string? EngagementDescription { get; set; } //allowed to be null

    public int EngagementTypeId { get; set; }

    public EngagementType EngagementType { get; set; } = null!;

    public int CommercialModelId { get; set; }

    public CommercialModel CommercialModel { get; set; } = null!;

    public DateOnly KickoffDate { get; set; }

    public DateOnly? BillingStartDate { get; set; }

    public int StatusId { get; set; }

    // Navigation Property
    public EngagementStatus Status { get; set; } = null!;

    public Models.EngagementChecklist.EngagementChecklist? Checklist { get; set; }

    public Pdr.Api.Models.Handover.HandoverInfo? HandoverInfo { get; set; }

    public ICollection<Pdr.Api.Models.EngagementAssignment.EngagementAssignment> EngagementAssignments { get; set; } = new List<Pdr.Api.Models.EngagementAssignment.EngagementAssignment>();
}