using Pdr.Api.DTOs.Auth;

namespace Pdr.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginRequestDto request);
}

public class AuthResultDto
{
    public bool IsSuccess { get; set; }
    public bool IsLockedOut { get; set; }
    public string? Token { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
