using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class RealisticCommitment : BaseAuditableEntity
{
    [Key]
    public long CommitmentId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int Timeline { get; set; }

    [Required]
    [MaxLength(20)]
    public string TimelineUnit { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? AcknowledgementMessage { get; set; }

    public bool IsPossible { get; set; } = true;
}
