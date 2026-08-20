namespace Pdr.Api.DTOs.Me;

public class MeDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTimeOffset? LastLoginAt { get; set; }
}
