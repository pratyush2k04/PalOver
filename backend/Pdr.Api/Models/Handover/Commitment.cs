using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class Commitment : BaseAuditableEntity
{
    [Key]
    public int CommitmentId { get; set; }

    [Required]
    public int HandoverId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CommitmentTypeId { get; set; }

    [Required]
    public int Timeline { get; set; }

    [Required]
    [MaxLength(20)]
    public string TimelineUnit { get; set; } = string.Empty;
}
