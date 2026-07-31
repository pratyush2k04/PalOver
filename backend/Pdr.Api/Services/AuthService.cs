using Pdr.Api.DTOs.Auth;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutDurationMinutes = 15;

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Your email or password is incorrect."
            };
        }

        // Check if user is currently locked out
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            return new AuthResultDto
            {
                IsSuccess = false,
                IsLockedOut = true,
                ErrorMessage = "Too many attempts. Try again in 15 minutes or ask your Admin to reset your password."
            };
        }

        // Validate password
        if (user.Password != request.Password)
        {
            user.FailedAttemptCount++;

            if (user.FailedAttemptCount >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(LockoutDurationMinutes);
                await userRepository.UpdateAsync(user);

                return new AuthResultDto
                {
                    IsSuccess = false,
                    IsLockedOut = true,
                    ErrorMessage = "Too many attempts. Try again in 15 minutes or ask your Admin to reset your password."
                };
            }

            await userRepository.UpdateAsync(user);

            return new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Your email or password is incorrect."
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
        return new AuthResultDto
        {
            IsSuccess = true,
            Token = token
        };
    }
}
