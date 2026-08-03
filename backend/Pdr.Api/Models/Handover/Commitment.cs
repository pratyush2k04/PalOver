using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class Commitment : BaseAuditableEntity
{
    [Key]
    public long CommitmentId { get; set; }

    [Required]
    public long HandoverId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CommitmentTypeId { get; set; }

    public string? Quantity { get; set; }
}
