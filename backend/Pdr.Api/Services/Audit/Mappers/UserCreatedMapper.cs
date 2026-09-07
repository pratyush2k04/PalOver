using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserCreatedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserCreated);

    private record UserCreatedData(int UserId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserCreatedData>(data)
            ?? throw new InvalidOperationException("UserCreated audit payload could not be parsed.");

        var user = await userRepository.GetByIdAsync(payload.UserId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName,
            ["actorName"] = actor?.FullName
        };
    }
}
