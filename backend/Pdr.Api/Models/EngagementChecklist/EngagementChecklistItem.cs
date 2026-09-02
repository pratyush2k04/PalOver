using Pdr.Api.Models;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Models.People;

namespace Pdr.Api.Models.EngagementChecklist;

public class EngagementChecklistItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int EngagementChecklistId { get; set; }
    public int? SourceMasterItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public bool IsMandatory { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool IsApplicable { get; set; } = true;
    public string Status { get; set; } = "Incomplete"; // "Incomplete" | "Complete" | "Blocked"

    // Applicability exemption fields
    public string? NotApplicableReason { get; set; }
    public string ExemptionStatus { get; set; } = "None"; // "None" | "Pending" | "Approved" | "Rejected"
    public int? ExemptionRequestedById { get; set; }
    public DateTime? ExemptionRequestedAt { get; set; }
    public int? ExemptionReviewedById { get; set; }
    public DateTime? ExemptionReviewedAt { get; set; }
    public string? ExemptionRejectionReason { get; set; }

    // Owner assignment
    public int? OwnerId { get; set; }

    // Blocking
    public string? BlockReason { get; set; }

    // Evidence tracking (FTR-026 / US-045)
    public bool EvidenceRequired { get; set; } = false;
    public int? EvidenceDocumentId { get; set; }
    public DateTime? EvidenceSubmittedAt { get; set; }
    public int? EvidenceSubmittedById { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public User? EvidenceSubmittedBy { get; set; }
    public Document? EvidenceDocument { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public EngagementChecklist EngagementChecklist { get; set; } = null!;
    public MasterChecklistCategory Category { get; set; } = null!;
}
