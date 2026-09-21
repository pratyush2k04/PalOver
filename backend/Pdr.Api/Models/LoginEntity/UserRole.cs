global using Role = Pdr.Api.Models.LoginEntity.UserRole;

namespace Pdr.Api.Models.LoginEntity;

using Pdr.Api.Models;

public class UserRole : BaseAuditableEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<User> Users { get; set; } = new List<User>();
}
