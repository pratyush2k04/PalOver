using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pdr.Api.Models;

public class Person : BaseAuditableEntity
{
    [Key]
    public int PersonId { get; set; }

    [Required]
    public int EngagementId { get; set; }

    [ForeignKey(nameof(EngagementId))]
    public Engagement Engagement { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Role { get; set; } = string.Empty;

    [Required]
    public PersonType PersonType { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }
}
