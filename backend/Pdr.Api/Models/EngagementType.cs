using System.ComponentModel.DataAnnotations;
namespace Pdr.Api.Models;

public class EngagementType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
}