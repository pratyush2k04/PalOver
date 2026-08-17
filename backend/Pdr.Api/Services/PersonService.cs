using Pdr.Api.DTOs.People;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class PersonService(
    IPersonRepository personRepository,
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
