using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserUpdatedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.UserUpdated.TemplateKey;

    private record UserUpdatedData(int UserId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserUpdatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.UserUpdated.ParseErrorMessage);

        var user = await userRepository.GetByIdAsync(payload.UserId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.UserUpdated.UserNameKey] = user?.FullName,
            [AuditConstants.UserUpdated.ActorNameKey] = actor?.FullName
        };
    }
}
