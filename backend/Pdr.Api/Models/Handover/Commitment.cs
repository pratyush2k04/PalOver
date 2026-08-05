using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class Commitment : BaseAuditableEntity
{
    [Key]
    public int CommitmentId { get; set; }

    [Required]
    public int HandoverId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CommitmentTypeId { get; set; }

    public string? Quantity { get; set; }
}
