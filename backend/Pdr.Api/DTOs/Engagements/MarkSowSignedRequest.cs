namespace Pdr.Api.DTOs.Engagements;

public record MarkSowSignedRequest(
    string? ClientName,
    string? EngagementDescription,
    int EngagementTypeId,
    DateOnly KickoffDate,
    DateOnly? BillingStartDate);
