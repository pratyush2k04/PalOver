using Pdr.Api.DTOs.ClientContactTransitions;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ClientContactTransitionService(
    IClientContactTransitionRepository repository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : IClientContactTransitionService
{
    public async Task<ClientContactTransitionResponseDto> GetByEngagementIdAsync(int engagementId)
    {
        var transition = await repository.GetByEngagementIdAsync(engagementId);
        if (transition is null)
        {
            throw new ClientContactTransitionNotFoundException(engagementId);
        }

        return MapToDto(transition);
    }

    public async Task<List<ClientContactTransitionResponseDto>> GetAllByEngagementIdAsync(int engagementId)
    {
        var transitions = await repository.GetAllByEngagementIdAsync(engagementId);
        return transitions.Select(MapToDto).ToList();
    }

    public async Task<ClientContactTransitionResponseDto> SaveTransitionAsync(int engagementId, SaveClientContactTransitionRequest request, string? currentUserId)
    {
        var transition = new ClientContactTransition
        {
            EngagementId = engagementId,
            OutgoingContactName = request.OutgoingContactName.Trim(),
            OutgoingContactRole = request.OutgoingContactRole.Trim(),
            IncomingContactName = request.IncomingContactName.Trim(),
            IncomingContactRole = request.IncomingContactRole.Trim(),
            CreatedBy = currentUserId,
            UpdatedBy = currentUserId
        };

        await repository.AddAsync(transition);
        await repository.SaveChangesAsync();

        var actorUserId = int.TryParse(currentUserId, out var uid) ? uid : currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.ClientContactDetailsChanged,
            new Dictionary<string, object?>
            {
                ["EngagementId"] = engagementId,
                ["ClientName"] = request.IncomingContactName.Trim(),
                ["ActorUserId"] = actorUserId
            },
            actingUserId: actorUserId > 0 ? actorUserId : null);

        return MapToDto(transition);
    }

    private static ClientContactTransitionResponseDto MapToDto(ClientContactTransition transition) =>
        new()
        {
            ClientContactTransitionId = transition.ClientContactTransitionId,
            EngagementId = transition.EngagementId,
            OutgoingContactName = transition.OutgoingContactName,
            OutgoingContactRole = transition.OutgoingContactRole,
            IncomingContactName = transition.IncomingContactName,
            IncomingContactRole = transition.IncomingContactRole
        };
}
