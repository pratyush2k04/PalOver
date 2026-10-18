using Pdr.Api.Models.People;

namespace Pdr.Api.Repositories.Interfaces;

public interface IPersonRepository
{
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<IEnumerable<Person>> GetByEngagementIdAsync(int engagementId);
    Task<Person?> GetByIdAsync(int personId);
    Task<Person?> GetBySourceRoleAsync(int engagementId, int sourceRoleId);
    Task<Person?> GetByUserAndSourceRoleAsync(int engagementId, int userId, int sourceRoleId);
    Task AddAsync(Person person);
    void Update(Person person);
    void Delete(Person person);
    Task<Person?> GetByNameAndRoleAndEngagementAsync(int engagementId, string name, string role);
    Task<Person?> GetByNameAndEngagementAsync(int engagementId, string name);
    Task<IEnumerable<Person>> GetByNamesAndEngagementAsync(int engagementId, IEnumerable<string> names);
    Task<bool> SaveChangesAsync();}
