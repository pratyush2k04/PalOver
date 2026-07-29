using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace Pdr.Api.Models;
 
public class Engagement
{
    [Key]
    public long EngagementId { get; set; }
 
    [Required]
    [MaxLength(200)]
    public string ClientName { get; set; } = string.Empty;
 
    public string? EngagementDescription { get; set; } //allowed to be null
 
    [Required]
    [MaxLength(100)]
    public string Nature { get; set; } = string.Empty;
 
     [Required]
    public int EngagementTypeId { get; set; }

    [ForeignKey(nameof(EngagementTypeId))]
    public EngagementType EngagementType { get; set; } = null!;
    
    [Required]
    public DateOnly KickoffDate { get; set; }
 
    public DateOnly? BillingStartDate { get; set; }
 
    public long? MasterChecklistVersionId { get; set; }
 
    public int StatusId { get; set; }
 
    public long CreatedBy { get; set; }
 
    public DateTime CreatedOn { get; set; }
 
    public long? UpdatedBy { get; set; }
 
    public DateTime? UpdatedOn { get; set; }
 
    // Navigation Property
    public EngagementStatus Status { get; set; } = null!;
}