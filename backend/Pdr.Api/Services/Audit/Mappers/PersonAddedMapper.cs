using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class PersonAddedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.PersonAdded);

    private record PersonAddedData(int EngagementId, string PersonName, string RoleName, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PersonAddedData>(data)
            ?? throw new InvalidOperationException("PersonAdded audit payload could not be parsed.");

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["personName"] = payload.PersonName,
            ["roleName"] = payload.RoleName,
            ["actorName"] = actor?.FullName
        };
    }
}
