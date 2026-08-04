using System.ComponentModel.DataAnnotations;

namespace Pdr.Api.Models.Handover;

public class HandoverInfo : BaseAuditableEntity
{
    [Key]
    public long HandoverId { get; set; }

    [Required]
    public long EngagementId { get; set; }

    [Required]
    public int StatusId { get; set; }

    [MaxLength(2000)]
    public string? ClientBackground { get; set; }

    [Required]
    [MaxLength(2000)]
    public string BusinessProblem { get; set; } = string.Empty;

    [Required]
    public string ClientSentiment { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? DealOrigin { get; set; }

    public bool HasNoCommitments { get; set; } = false;
}
