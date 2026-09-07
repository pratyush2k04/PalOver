using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserCreatedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.UserCreated.TemplateKey;

    private record UserCreatedData(int UserId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserCreatedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.UserCreated.ParseErrorMessage);

        var user = await userRepository.GetByIdAsync(payload.UserId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.UserCreated.UserNameKey] = user?.FullName,
            [AuditConstants.UserCreated.ActorNameKey] = actor?.FullName
        };
    }
}
