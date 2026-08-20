using Pdr.Api.DTOs.Profile;

namespace Pdr.Api.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync();
    Task<ProfileDto> UpdateProfileAsync(UpdateProfileDto request);
    Task ChangePasswordAsync(ChangePasswordDto request);
}
