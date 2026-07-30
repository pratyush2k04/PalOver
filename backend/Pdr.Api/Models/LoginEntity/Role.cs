namespace Pdr.Api.Models.LoginEntity;

public class Role
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
