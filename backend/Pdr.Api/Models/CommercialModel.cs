using System.ComponentModel.DataAnnotations;
namespace Pdr.Api.Models;

public class CommercialModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
}
