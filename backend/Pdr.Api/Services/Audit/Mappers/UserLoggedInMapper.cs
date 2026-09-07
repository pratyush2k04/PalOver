using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserLoggedInMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.UserLoggedIn.TemplateKey;

    private record UserLoggedInData(int UserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<UserLoggedInData>(data)
            ?? throw new InvalidOperationException(AuditConstants.UserLoggedIn.ParseErrorMessage);

        var user = await userRepository.GetByIdAsync(payload.UserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.UserLoggedIn.UserNameKey] = user?.FullName
        };
    }
}
