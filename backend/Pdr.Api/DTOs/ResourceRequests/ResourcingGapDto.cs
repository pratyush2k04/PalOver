namespace Pdr.Api.DTOs.ResourceRequests;

public record ResourcingGapDto(
    bool HasGap,
    int UnconfirmedCount,
    int? DaysToBilling
);
