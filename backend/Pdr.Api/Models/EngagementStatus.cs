using System.ComponentModel.DataAnnotations;
 
namespace Pdr.Api.Models;
 
public class EngagementStatus
{
    [Key]
    public int StatusId { get; set; }
 
    [Required]
    [MaxLength(50)]
    public string StatusName { get; set; } = string.Empty;
 
    public ICollection<Engagement> Engagements { get; set; } =[];
     
}