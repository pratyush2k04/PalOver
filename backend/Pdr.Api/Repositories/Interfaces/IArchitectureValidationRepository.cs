using Pdr.Api.Models.ArchitectureValidation;

namespace Pdr.Api.Repositories.Interfaces;

public interface IArchitectureValidationRepository
{
    Task<ArchitectureValidation?> GetByEngagementIdAsync(int engagementId);
    Task<ArchitectureValidation?> GetByIdAsync(int id);
    Task<DiscoveryFeatureItem?> GetFeatureItemByIdAsync(int featureId);
    Task AddValidationAsync(ArchitectureValidation validation);
    Task AddFeatureItemAsync(DiscoveryFeatureItem item);
    Task<decimal> GetCommittedTimelineWeeksAsync(int engagementId);
    Task SaveChangesAsync();
    Task<bool> EngagementExistsAsync(int engagementId);}
