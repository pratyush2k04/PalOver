using Pdr.Api.DTOs.Profile;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ProfileService(
    IUserRepository userRepository,
    ICurrentUserService currentUserService) : IProfileService
{
    public async Task<ProfileDto> GetProfileAsync()
    {
        var user = await GetCurrentUserAsync();
        return ToDto(user);
    }

    public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileDto request)
    {
        var user = await GetCurrentUserAsync();

        user.FullName = request.FullName;
        user.Email = request.Email;

        await userRepository.UpdateAsync(user);

        return ToDto(user);
    }

    public async Task ChangePasswordAsync(ChangePasswordDto request)
    {
        var user = await GetCurrentUserAsync();

        if (user.Password != request.CurrentPassword)
        {
            throw new IncorrectCurrentPasswordException();
        }

        user.Password = request.NewPassword;
        await userRepository.UpdateAsync(user);
    }

    private async Task<User> GetCurrentUserAsync()
    {
        var userId = currentUserService.GetUserId();
        return await userRepository.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);
    }

    private static ProfileDto ToDto(User user) => new()
    {
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.Name,
        LastLoginAt = user.LastLoginAt
    };
}
