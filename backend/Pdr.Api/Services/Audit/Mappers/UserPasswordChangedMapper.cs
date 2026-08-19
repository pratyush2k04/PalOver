using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserPasswordChangedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserPasswordChanged);

    private record UserPasswordChangedData(int UserId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserPasswordChangedData>(data)
            ?? throw new InvalidOperationException("UserPasswordChanged audit payload could not be parsed.");

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["actorName"] = actor?.FullName
        };
    }
}
