namespace Pdr.Api.DTOs.Profile;

public class ChangePasswordDto
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
}
