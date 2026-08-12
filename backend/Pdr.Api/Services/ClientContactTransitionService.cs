using Pdr.Api.DTOs.ClientContactTransitions;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ClientContactTransitionService(IClientContactTransitionRepository repository) : IClientContactTransitionService
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

    public async Task<ClientContactTransitionResponseDto> SaveTransitionAsync(int engagementId, SaveClientContactTransitionRequest request, string? currentUserId)
    {
        var outgoingName = request.OutgoingContactName.Trim();
        var outgoingRole = request.OutgoingContactRole.Trim();
        var incomingName = request.IncomingContactName.Trim();
        var incomingRole = request.IncomingContactRole.Trim();

        var existingTransition = await repository.GetByEngagementIdAsync(engagementId);

        if (existingTransition is not null)
        {
            existingTransition.OutgoingContactName = outgoingName;
            existingTransition.OutgoingContactRole = outgoingRole;
            existingTransition.IncomingContactName = incomingName;
            existingTransition.IncomingContactRole = incomingRole;
            existingTransition.UpdatedBy = currentUserId;

            repository.Update(existingTransition);
            await repository.SaveChangesAsync();

            return MapToDto(existingTransition);
        }

        var transition = new ClientContactTransition
        {
            EngagementId = engagementId,
            OutgoingContactName = outgoingName,
            OutgoingContactRole = outgoingRole,
            IncomingContactName = incomingName,
            IncomingContactRole = incomingRole,
            CreatedBy = currentUserId,
            UpdatedBy = currentUserId
        };

        await repository.AddAsync(transition);
        await repository.SaveChangesAsync();

        return MapToDto(transition);
    }

    public async Task<CurrentClientContactDto> GetCurrentClientContactAsync(int engagementId)
    {
        var transition = await repository.GetByEngagementIdAsync(engagementId);
        if (transition is null)
        {
            return new CurrentClientContactDto();
        }

        return new CurrentClientContactDto
        {
            Name = transition.IncomingContactName,
            Role = transition.IncomingContactRole
        };
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
