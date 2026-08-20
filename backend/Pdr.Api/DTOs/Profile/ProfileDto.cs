namespace Pdr.Api.DTOs.Profile;

public class ProfileDto
{
    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required string Role { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }
}
