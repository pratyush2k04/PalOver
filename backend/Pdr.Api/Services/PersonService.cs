using Pdr.Api.DTOs.People;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.People;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class PersonService(
    
    IPersonRepository personRepository,
    IClientContactTransitionRepository transitionRepository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : IPersonService
{
    public async Task<IEnumerable<PersonDto>> GetPeopleByEngagementAsync(int engagementId)
    {
        var exists = await personRepository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var people = await personRepository.GetByEngagementIdAsync(engagementId);
        return people.Select(MapToDto);
    }

    public async Task<PersonDto> CreatePersonAsync(int engagementId, PersonDto dto, string? currentUserId)
    {
        var exists = await personRepository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var person = new Person
        {
            EngagementId = engagementId,
            Name = dto.Name.Trim(),
            Role = dto.Role.Trim(),
            PersonType = dto.PersonType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId,
            UpdatedBy = currentUserId
        };

        await personRepository.AddAsync(person);
        await personRepository.SaveChangesAsync();

        if (person.PersonType == PersonType.Client)
        {
            var actorUserId = int.TryParse(currentUserId, out var uid) ? uid : currentUserService.GetUserId();
            await auditLogService.LogAsync(
                AuditActionEnum.ClientContactDetailsChanged,
                new Dictionary<string, object?>
                {
                    ["EngagementId"] = engagementId,
                    ["ClientName"] = person.Name,
                    ["ActorUserId"] = actorUserId
                },
                engagementId: engagementId,
                actingUserId: actorUserId > 0 ? actorUserId : null);
        }

        return MapToDto(person);
    }

    public async Task<PersonDto> UpdatePersonAsync(int engagementId, int personId, PersonDto dto, string? currentUserId)
    {
        var exists = await personRepository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var person = await personRepository.GetByIdAsync(personId);
        if (person is null || person.EngagementId != engagementId)
        {
            throw new PersonNotFoundException(personId, engagementId);
        }

        var oldName = person.Name;
        var oldRole = person.Role;
        var isClient = person.PersonType == PersonType.Client || dto.PersonType == PersonType.Client;

        person.Name = dto.Name.Trim();
        person.Role = dto.Role.Trim();
        person.PersonType = dto.PersonType;
        person.UpdatedAt = DateTime.UtcNow;
        person.UpdatedBy = currentUserId;

        personRepository.Update(person);
        await personRepository.SaveChangesAsync();

        if (isClient)
        {
            var actorUserId = int.TryParse(currentUserId, out var uid) ? uid : currentUserService.GetUserId();

            if (!string.Equals(oldRole, person.Role, StringComparison.OrdinalIgnoreCase))
            {
                await auditLogService.LogAsync(
                    AuditActionEnum.ClientPersonaChanged,
                    new Dictionary<string, object?>
                    {
                        ["EngagementId"] = engagementId,
                        ["ClientName"] = person.Name,
                        ["Persona"] = person.Role,
                        ["ActorUserId"] = actorUserId
                    },
                    engagementId: engagementId,
                    actingUserId: actorUserId > 0 ? actorUserId : null);
            }

            if (!string.Equals(oldName, person.Name, StringComparison.OrdinalIgnoreCase))
            {
                await auditLogService.LogAsync(
                    AuditActionEnum.ClientContactDetailsChanged,
                    new Dictionary<string, object?>
                    {
                        ["EngagementId"] = engagementId,
                        ["ClientName"] = person.Name,
                        ["ActorUserId"] = actorUserId
                    },
                    engagementId: engagementId,
                    actingUserId: actorUserId > 0 ? actorUserId : null);
            }
        }

        return MapToDto(person);
    }

    public async Task DeletePersonAsync(int engagementId, int personId, string? currentUserId)
    {
        var exists = await personRepository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var person = await personRepository.GetByIdAsync(personId);
        if (person is null || person.EngagementId != engagementId)
        {
            throw new PersonNotFoundException(personId, engagementId);
        }

        person.UpdatedBy = currentUserId;
        personRepository.Delete(person);
        await personRepository.SaveChangesAsync();
    }

    public async Task<ImportHandoverContactsResponseDto> ImportHandoverContactsAsync(int engagementId, string? currentUserId)
    {
        var exists = await personRepository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var transitions = await transitionRepository.GetAllByEngagementIdAsync(engagementId);
        if (!transitions.Any())
        {
            return new ImportHandoverContactsResponseDto { Added = 0, AlreadyPresent = 0, Deleted = 0 };
        }

        int added = 0, alreadyPresent = 0, deleted = 0;

        // For each outgoing contact, only the latest transition naming them as outgoing
        // reflects the current handover state - earlier transitions for the same outgoing
        // contact are superseded and their incoming contact is discarded.
        var latestPerOutgoing = transitions
            .Where(t => !string.IsNullOrEmpty(t.OutgoingContactName.Trim()))
            .GroupBy(t => t.OutgoingContactName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(t => t.CreatedAt).First());

        var incomingContacts = latestPerOutgoing
            .Select(t => new { Name = t.IncomingContactName.Trim(), Role = t.IncomingContactRole.Trim() })
            .Where(c => !string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Role))
            .DistinctBy(c => (c.Name.ToLower(), c.Role.ToLower()));

        foreach (var incoming in incomingContacts)
        {
            var existingPerson = await personRepository.GetByNameAndRoleAndEngagementAsync(engagementId, incoming.Name, incoming.Role);
            if (existingPerson is null)
            {
                var newPerson = new Person
                {
                    EngagementId = engagementId,
                    Name = incoming.Name,
                    Role = incoming.Role,
                    PersonType = PersonType.Client,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId,
                    UpdatedBy = currentUserId
                };
                await personRepository.AddAsync(newPerson);
                added++;
            }
            else
            {
                alreadyPresent++;
            }
        }

        var outgoingNames = transitions
            .Select(t => t.OutgoingContactName.Trim())
            .Where(name => !string.IsNullOrEmpty(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (outgoingNames.Any())
        {
            var peopleToDelete = await personRepository.GetByNamesAndEngagementAsync(engagementId, outgoingNames);
            foreach (var personToDelete in peopleToDelete)
            {
                personToDelete.UpdatedBy = currentUserId;
                personRepository.Delete(personToDelete);
                deleted++;
            }
        }

        await personRepository.SaveChangesAsync();

        return new ImportHandoverContactsResponseDto
        {
            Added = added,
            AlreadyPresent = alreadyPresent,
            Deleted = deleted
        };
    }

    private static PersonDto MapToDto(Person person)
    {
        return new PersonDto
        {
            PersonId = person.PersonId,
            EngagementId = person.EngagementId,
            Name = person.Name,
            Role = person.Role,
            PersonType = person.PersonType
        };
    }
}
