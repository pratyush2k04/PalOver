using Pdr.Api.Constants;
using Pdr.Api.DTOs.Auth;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IAuditLogService auditLogService) : IAuthService
{
    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = AuthConstants.Messages.InvalidCredentials
            };
        }

        // Check if user is currently locked out
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            return new AuthResultDto
            {
                IsSuccess = false,
                IsLockedOut = true,
                ErrorMessage = AuthConstants.Messages.AccountLockedOut
            };
        }

        // Validate password
        if (user.Password != request.Password)
        {
            user.FailedAttemptCount++;

            if (user.FailedAttemptCount >= AuthConstants.MaxFailedAttempts)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(AuthConstants.LockoutDurationMinutes);
                await userRepository.UpdateAsync(user);

                return new AuthResultDto
                {
                    IsSuccess = false,
                    IsLockedOut = true,
                    ErrorMessage = AuthConstants.Messages.AccountLockedOut
                };
            }

            await userRepository.UpdateAsync(user);

            return new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = AuthConstants.Messages.InvalidCredentials
            };
        }

        // Credentials valid -> Reset failed attempts and lockout
        if (user.FailedAttemptCount > 0 || user.LockoutEnd.HasValue)
        {
            user.FailedAttemptCount = 0;
            user.LockoutEnd = null;
            await userRepository.UpdateAsync(user);
        }

        var token = jwtTokenGenerator.GenerateToken(user);

        await auditLogService.LogAsync(
            AuditActionEnum.UserLoggedIn,
            new Dictionary<string, object?> { ["UserId"] = user.Id },
            actingUserId: user.Id);

        return new AuthResultDto
        {
            IsSuccess = true,
            Token = token
        };
    }
}
