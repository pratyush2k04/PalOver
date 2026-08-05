namespace Pdr.Api.Models.LoginEntity;

using Pdr.Api.Models;

public class Role : BaseAuditableEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<User> Users { get; set; } = new List<User>();
}
