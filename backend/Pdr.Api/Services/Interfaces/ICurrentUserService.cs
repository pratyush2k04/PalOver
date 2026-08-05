namespace Pdr.Api.Services.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string? GetRole();
    string? GetEmail();
    string? GetFullName();
    bool IsInRole(string role);
    bool IsAdmin();
}
