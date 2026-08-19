using Pdr.Api.DTOs.Me;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class MeService(
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IAuditLogService auditLogService) : IMeService
{
    public async Task<MeDto> GetMeAsync()
    {
        var user = await GetCurrentUserAsync();
        return ToDto(user);
    }

    public async Task<MeDto> UpdateMeAsync(UpdateMeDto request)
    {
        var user = await GetCurrentUserAsync();

        var changedFields = new List<string>();

        if (!string.Equals(user.FullName, request.FullName, StringComparison.Ordinal))
        {
            user.FullName = request.FullName;
            changedFields.Add(nameof(User.FullName));
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = request.Email;
            changedFields.Add(nameof(User.Email));
        }

        if (changedFields.Count > 0)
        {
            await userRepository.UpdateAsync(user);

            await auditLogService.LogAsync(AuditActionEnum.UserProfileUpdated, new Dictionary<string, object?>
            {
                ["UserId"] = user.Id,
                ["Field"] = string.Join(", ", changedFields),
                ["ActorUserId"] = currentUserService.GetUserId()
            });
        }

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

        await auditLogService.LogAsync(AuditActionEnum.UserPasswordChanged, new Dictionary<string, object?>
        {
            ["UserId"] = user.Id,
            ["ActorUserId"] = currentUserService.GetUserId()
        });
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
