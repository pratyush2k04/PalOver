using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class HandoverStatus
{
    [Key]
    public int HandoverStatusId { get; set; }

    [Required]
    public string StatusName { get; set; } = string.Empty;
}
