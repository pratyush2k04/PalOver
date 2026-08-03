namespace Pdr.Api.DTOs.Engagements;

public record PmEngagementListItemDto(
    long EngagementId,
    string ClientName,
    string EngagementTypeName,
    string CommercialModelName,
    DateOnly KickoffDate,
    int DaysToKickoff,
    int? ReadinessScore);

public record PmEngagementListResponse(List<PmEngagementListItemDto> Engagements);
