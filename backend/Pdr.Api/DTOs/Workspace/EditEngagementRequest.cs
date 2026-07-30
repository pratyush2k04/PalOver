namespace Pdr.Api.DTOs.Workspace;

public record EditEngagementRequest(
    string? ClientName,
    string? EngagementDescription,
    int? EngagementTypeId,
    DateOnly? KickoffDate,
    DateOnly? BillingStartDate);
