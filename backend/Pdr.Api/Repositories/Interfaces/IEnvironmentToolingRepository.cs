using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEnvironmentToolingRepository
{
    Task<List<EnvironmentToolingItem>> GetByEngagementIdAsync(int engagementId);
    Task<EnvironmentToolingItem?> GetByIdAsync(int id);
    Task AddAsync(EnvironmentToolingItem item);
    Task SaveChangesAsync();

    Task DeleteAsync(EnvironmentToolingItem item);

    // Validation lookups (self-contained — no service touches DbContext directly)
    Task<bool> EngagementExistsAsync(int engagementId);
    Task<bool> UserExistsAsync(int userId);}
