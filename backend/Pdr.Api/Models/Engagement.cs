using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Models.EngagementAssignment;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Pdr.Api.Models;
 
public class Engagement : BaseAuditableEntity
{
    [Key]
    public int EngagementId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string EngagementName { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string ClientName { get; set; } = string.Empty;

    public string? EngagementDescription { get; set; } //allowed to be null

    [Required]
    public int EngagementTypeId { get; set; }

    [ForeignKey(nameof(EngagementTypeId))]
    public EngagementType EngagementType { get; set; } = null!;

    
    [Required]
    public int CommercialModelId { get; set; }

    [ForeignKey(nameof(CommercialModelId))]
    public CommercialModel CommercialModel { get; set; } = null!;

    [Required]
    public DateOnly KickoffDate { get; set; }

    public DateOnly? BillingStartDate { get; set; }

    public int StatusId { get; set; }

    // Navigation Property
    public EngagementStatus Status { get; set; } = null!;

    public int? ChecklistVersionId { get; set; }

    public MasterChecklist.MasterChecklistVersion? ChecklistVersion { get; set; }

    public Handover.HandoverInfo? HandoverInfo { get; set; }

    //Technical Readiness
    public ICollection<EngagementAssignment.EngagementAssignment> EngagementAssignments { get; set; } = new List<EngagementAssignment.EngagementAssignment>();

    public ICollection<AccessItem> AccessItems { get; set; }
    = new List<AccessItem>();

    public ICollection<EnvironmentToolingItem> EnvironmentToolingItems { get; set; }
        = new List<EnvironmentToolingItem>();

    public ICollection<Pdr.Api.Models.Risk.Risk> Risks { get; set; }
        = new List<Pdr.Api.Models.Risk.Risk>();
}
