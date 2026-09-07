using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserDeletedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserDeleted);

    private record UserDeletedData(int UserId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserDeletedData>(data)
            ?? throw new InvalidOperationException("UserDeleted audit payload could not be parsed.");

        var user = await userRepository.GetByIdAsync(payload.UserId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName,
            ["actorName"] = actor?.FullName
        };
    }
}
