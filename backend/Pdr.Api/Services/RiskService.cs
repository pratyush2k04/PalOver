using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

/// <summary>
/// Stub implementation of IRiskService.
/// All methods are no-ops until the Risk module developer implements them.
///
/// IMPLEMENTATION NOTES FOR RISK DEVELOPER:
/// ------------------------------------------
/// - Inject AppDbContext (or IRiskRepository once created) via constructor.
/// - Risk model fields available: RiskId, EngagementId, Title, Description, Status (Open/Closed).
/// - All methods that create risks should guard against duplicate Open risks
///   for the same (EngagementId + Title) combination.
/// - Use RiskStatus.Open when creating, RiskStatus.Closed when resolving.
/// </summary>
public class RiskService : IRiskService
{
    /// <summary>
    /// TODO: Implement risk creation.
    /// - Query Risks where EngagementId == engagementId AND Title == title AND Status == Open.
    /// - If none found, insert a new Risk with Status = Open.
    /// - SaveChanges.
    /// </summary>
    public Task CreateRiskAsync(int engagementId, string title, string description)
    {
        // Stub — to be implemented by Risk module developer
        return Task.CompletedTask;
    }

    /// <summary>
    /// TODO: Implement risk resolution.
    /// - Query Risks where EngagementId == engagementId AND Title == title AND Status == Open.
    /// - If found, set Status = Closed.
    /// - SaveChanges.
    /// </summary>
    public Task ResolveRiskAsync(int engagementId, string title)
    {
        // Stub — to be implemented by Risk module developer
        return Task.CompletedTask;
    }

    /// <summary>
    /// TODO: Implement client-side billing risk creation (User Story 3).
    /// - Title format: $"Client Access Not Granted: {accessItemName}"
    /// - Description format: $"The client-side access item '{accessItemName}' has not been granted
    ///   and billing is starting within 2 days."
    /// - Guard against duplicate Open risks for the same (EngagementId + Title).
    /// - SaveChanges.
    /// </summary>
    public Task CreateClientSideRiskAsync(int engagementId, string accessItemName)
    {
        // Stub — to be implemented by Risk module developer
        return Task.CompletedTask;
    }
}
