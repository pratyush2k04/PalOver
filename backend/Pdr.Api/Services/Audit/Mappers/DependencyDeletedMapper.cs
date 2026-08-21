using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DependencyDeletedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DependencyDeleted);

    private record PayloadData(int EngagementId, string DependencyDescription, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PayloadData>(data)
            ?? throw new InvalidOperationException("DependencyDeleted audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["dependencyDescription"] = payload.DependencyDescription,
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
