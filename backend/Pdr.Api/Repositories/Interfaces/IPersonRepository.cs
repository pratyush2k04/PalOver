using Pdr.Api.Models;

namespace Pdr.Api.Repositories.Interfaces;

public interface IPersonRepository
{
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<IEnumerable<Person>> GetByEngagementIdAsync(int engagementId);
    Task<Person?> GetByIdAsync(int personId);
    Task AddAsync(Person person);
    void Update(Person person);
    void Delete(Person person);
    Task<Person?> GetByNameAndEngagementAsync(int engagementId, string name);
    Task<bool> SaveChangesAsync();
}
