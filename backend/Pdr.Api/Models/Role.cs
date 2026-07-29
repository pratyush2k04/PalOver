namespace Pdr.Api.Models;

public class Role : BaseAuditableEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
