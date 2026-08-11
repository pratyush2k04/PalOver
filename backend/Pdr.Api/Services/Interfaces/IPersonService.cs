using Pdr.Api.DTOs.People;

namespace Pdr.Api.Services.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonDto>> GetPeopleByEngagementAsync(int engagementId);
    Task<PersonDto> CreatePersonAsync(int engagementId, PersonDto dto, string? currentUserId);
    Task<PersonDto> UpdatePersonAsync(int engagementId, int personId, PersonDto dto, string? currentUserId);
    Task DeletePersonAsync(int engagementId, int personId, string? currentUserId);
}
