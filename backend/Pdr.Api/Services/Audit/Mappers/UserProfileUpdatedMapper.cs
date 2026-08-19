using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserProfileUpdatedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserProfileUpdated);

    private record UserProfileUpdatedData(int UserId, string Field, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserProfileUpdatedData>(data)
            ?? throw new InvalidOperationException("UserProfileUpdated audit payload could not be parsed.");

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["field"] = payload.Field,
            ["actorName"] = actor?.FullName
        };
    }
}
