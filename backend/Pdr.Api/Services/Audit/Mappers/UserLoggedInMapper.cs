using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserLoggedInMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserLoggedIn);

    private record UserLoggedInData(int UserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserLoggedInData>(data)
            ?? throw new InvalidOperationException("UserLoggedIn audit payload could not be parsed.");

        var user = await userRepository.GetByIdAsync(payload.UserId);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName
        };
    }
}
