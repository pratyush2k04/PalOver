using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Models.EngagementAssignment;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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

    public EngagementChecklist.EngagementChecklist? Checklist { get; set; }

    public Handover.HandoverInfo? HandoverInfo { get; set; }

    public ICollection<EngagementAssignment.EngagementAssignment> EngagementAssignments { get; set; } = new List<EngagementAssignment.EngagementAssignment>();
    //Technical Readiness

    public ICollection<AccessItem> AccessItems { get; set; }
    = new List<AccessItem>();

    public ICollection<EnvironmentToolingItem> EnvironmentToolingItems { get; set; }
        = new List<EnvironmentToolingItem>();
}
