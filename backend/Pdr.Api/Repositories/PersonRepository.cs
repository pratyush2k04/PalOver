using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.People;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Repositories;

public class PersonRepository(AppDbContext context) : IPersonRepository
{
    public async Task<bool> EngagementExistsAsync(int engagementId)
    {
        return await context.Set<Engagement>().AnyAsync(e => e.EngagementId == engagementId);
    }

    public async Task<IEnumerable<Person>> GetByEngagementIdAsync(int engagementId)
    {
        return await context.People
            .Where(p => p.EngagementId == engagementId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Person?> GetByIdAsync(int personId)
    {
        return await context.People
            .FirstOrDefaultAsync(p => p.PersonId == personId);
    }

    public async Task<Person?> GetBySourceRoleAsync(int engagementId, int sourceRoleId)
    {
        return await context.People
            .FirstOrDefaultAsync(p => p.EngagementId == engagementId && p.SourceRoleId == sourceRoleId);
    }

    public async Task<Person?> GetByUserAndSourceRoleAsync(int engagementId, int userId, int sourceRoleId)
    {
        return await context.People
            .FirstOrDefaultAsync(p => p.EngagementId == engagementId && p.UserId == userId && p.SourceRoleId == sourceRoleId);
    public async Task<Person?> GetByNameAndRoleAndEngagementAsync(int engagementId, string name, string role)
    {
        return await context.People
            .FirstOrDefaultAsync(p => p.EngagementId == engagementId
                && p.Name.ToLower() == name.ToLower()
                && p.Role.ToLower() == role.ToLower());
    }

    public async Task<IEnumerable<Person>> GetByNamesAndEngagementAsync(int engagementId, IEnumerable<string> names)
    {
        var lowerNames = names.Select(n => n.ToLower()).ToList();
        return await context.People
            .Where(p => p.EngagementId == engagementId && lowerNames.Contains(p.Name.ToLower()))
            .ToListAsync();
    }

    public async Task AddAsync(Person person)
    {
        await context.People.AddAsync(person);
    }

    public void Update(Person person)
    {
        context.People.Update(person);
    }

    public void Delete(Person person)
    {
        // Soft delete
        person.IsDeleted = true;
        person.UpdatedAt = DateTime.UtcNow;
        context.People.Update(person);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
