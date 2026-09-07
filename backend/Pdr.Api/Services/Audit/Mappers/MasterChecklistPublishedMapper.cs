using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class MasterChecklistPublishedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.MasterChecklistPublished);

    private record MasterChecklistPublishedData(int Version, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<MasterChecklistPublishedData>(data)
            ?? throw new InvalidOperationException("MasterChecklistPublished audit payload could not be parsed.");

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["version"] = payload.Version.ToString(),
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
