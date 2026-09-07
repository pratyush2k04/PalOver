using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class AccountLockoutMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.AccountLockout.TemplateKey;

    private record AccountLockoutData(int UserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<AccountLockoutData>(data)
            ?? throw new InvalidOperationException(AuditConstants.AccountLockout.ParseErrorMessage);

        var user = await userRepository.GetByIdAsync(payload.UserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.AccountLockout.UserNameKey] = user?.FullName ?? user?.Email ?? "Unknown User"
        };
    }
}
