using Pdr.Api.Models.People;

namespace Pdr.Api.DTOs.People;

public class PersonDto
{
    public int PersonId { get; set; }
    public int EngagementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public PersonType PersonType { get; set; }
    public int? UserId { get; set; }
}
