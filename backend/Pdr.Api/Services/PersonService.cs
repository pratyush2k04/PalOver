using Pdr.Api.DTOs.People;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class PersonService(
    IPersonRepository personRepository,
    IClientContactTransitionRepository transitionRepository) : IPersonService
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

        person.Name = dto.Name.Trim();
        person.Role = dto.Role.Trim();
        person.PersonType = dto.PersonType;
        person.UpdatedAt = DateTime.UtcNow;
        person.UpdatedBy = currentUserId;

        personRepository.Update(person);
        await personRepository.SaveChangesAsync();

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

        var latestTransition = transitions.OrderByDescending(t => t.CreatedAt).First();
        var incomingName = latestTransition.IncomingContactName.Trim();

        int added = 0, alreadyPresent = 0, deleted = 0;

        if (!string.IsNullOrEmpty(incomingName))
        {
            var existingPerson = await personRepository.GetByNameAndEngagementAsync(engagementId, incomingName);
            if (existingPerson is null)
            {
                var newPerson = new Person
                {
                    EngagementId = engagementId,
                    Name = incomingName,
                    Role = latestTransition.IncomingContactRole.Trim(),
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
