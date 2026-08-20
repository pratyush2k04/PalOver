using Pdr.Api.DTOs.Me;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class MeService(
    IUserRepository userRepository,
    ICurrentUserService currentUserService) : IMeService
{
    public async Task<MeDto> GetMeAsync()
    {
        var user = await GetCurrentUserAsync();
        return ToDto(user);
    }

    public async Task<MeDto> UpdateMeAsync(UpdateMeDto request)
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

    private static MeDto ToDto(User user) => new()
    {
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.Name,
        LastLoginAt = user.LastLoginAt
    };
}
