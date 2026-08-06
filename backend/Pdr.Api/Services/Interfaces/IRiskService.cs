namespace Pdr.Api.Services.Interfaces;

/// <summary>
/// Placeholder interface for the Risk module.
/// To be fully implemented by the Risk module developer.
///
/// PLAN OF ACTION FOR RISK DEVELOPER:
/// -------------------------------------
/// 1. CreateRiskAsync:
///    - Check if an Open risk with the same title already exists for the engagement.
///    - If not, create a new Risk record with Status = Open.
///    - Link the risk to the engagement via EngagementId.
///
/// 2. ResolveRiskAsync:
///    - Find the Open risk by engagement + title.
///    - Set its Status = Closed.
///    - SaveChanges.
///
/// 3. CreateClientSideRiskAsync:
///    - Same as CreateRiskAsync but with a standard title/description format
///      indicating this was auto-triggered by the 2-day billing window rule (User Story 3).
///    - Guard against duplicate open risks for the same item name.
/// </summary>
public interface IRiskService
{
    /// <summary>
    /// Called when an AccessItem or EnvironmentToolingItem is blocked.
    /// Creates an Open risk linked to the engagement.
    /// </summary>
    Task CreateRiskAsync(int engagementId, string title, string description);

    /// <summary>
    /// Called when a blocked item is unblocked.
    /// Finds the matching Open risk by title and closes it.
    /// </summary>
    Task ResolveRiskAsync(int engagementId, string title);

    /// <summary>
    /// Called by the daily background job and the test evaluation API (User Story 3).
    /// Triggered when a client-side AccessItem is not Granted and
    /// BillingStartDate is within 2 days of the deadline.
    /// </summary>
    Task CreateClientSideRiskAsync(int engagementId, string accessItemName);
}
