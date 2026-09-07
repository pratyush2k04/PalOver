using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class MasterChecklistPublishedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.MasterChecklistPublished.TemplateKey;

    private record MasterChecklistPublishedData(int Version, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<MasterChecklistPublishedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.MasterChecklistPublished.ParseErrorMessage);

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.MasterChecklistPublished.VersionKey] = payload.Version.ToString(),
            [AuditConstants.MasterChecklistPublished.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
