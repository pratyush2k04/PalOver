namespace Pdr.Api.DTOs.Engagements;

public record EngagementOwnersDto(
    int? ProjectManagerId,
    string? ProjectManagerName,
    int? TechLeadId,
    string? TechLeadName);
