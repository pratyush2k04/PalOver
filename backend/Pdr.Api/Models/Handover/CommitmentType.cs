using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class CommitmentType
{
    [Key]
    public int CommitmentTypeId { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;
}
