using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class PersonAddedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.PersonAdded.TemplateKey;

    private record PersonAddedData(int EngagementId, string PersonName, string RoleName, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PersonAddedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.PersonAdded.ParseErrorMessage);

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.PersonAdded.PersonNameKey] = payload.PersonName,
            [AuditConstants.PersonAdded.RoleNameKey] = payload.RoleName,
            [AuditConstants.PersonAdded.ActorNameKey] = actor?.FullName
        };
    }
}
