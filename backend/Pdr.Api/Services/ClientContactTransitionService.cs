using Pdr.Api.DTOs.ClientContactTransitions;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ClientContactTransitionService(
    IClientContactTransitionRepository repository,
    IEngagementRepository engagementRepository,
    IAuditLogService auditLogService,
    IPersonRepository personRepository,
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
        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

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

        var outgoingPerson = await personRepository.GetByNameAndRoleAndEngagementAsync(
            engagementId, request.OutgoingContactName.Trim(), request.OutgoingContactRole.Trim());

        if (outgoingPerson != null)
        {
            outgoingPerson.Name = request.IncomingContactName.Trim();
            outgoingPerson.Role = request.IncomingContactRole.Trim();
            outgoingPerson.UpdatedAt = DateTime.UtcNow;
            outgoingPerson.UpdatedBy = currentUserId;
            personRepository.Update(outgoingPerson);
            await personRepository.SaveChangesAsync();
        }

        var actorUserId = int.TryParse(currentUserId, out var uid) ? uid : currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.ClientContactTransitioned,
            new Dictionary<string, object?>
            {
                ["EngagementId"] = engagementId,
                ["OutgoingName"] = request.OutgoingContactName.Trim(),
                ["IncomingName"] = request.IncomingContactName.Trim(),
                ["ActorUserId"] = actorUserId
            },
            engagementId: engagementId,
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
