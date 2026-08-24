namespace Pdr.Api.DTOs.Kpi;

public enum KpiPeriodStatus
{
    Ok = 0,
    NoEngagementsInPeriod = 1
}

public record ReadinessTimeValueDto(double? AverageDays, KpiPeriodStatus Status);

public record ReadinessTimeKpiDto(
    ReadinessTimeValueDto Current,
    ReadinessTimeValueDto? Prior,
    double? AbsoluteChangeDays,
    bool HasPriorPeriodComparison);

public record KpiAverageDaysValueDto(double? AverageDays, KpiPeriodStatus Status);

public record AccessDelayKpiDto(
    KpiAverageDaysValueDto Current,
    KpiAverageDaysValueDto? Prior,
    double? AbsoluteChangeDays,
    bool HasPriorPeriodComparison);

public record KpiCountValueDto(int? Count, KpiPeriodStatus Status);

public record EscalationCountKpiDto(
    KpiCountValueDto Current,
    KpiCountValueDto? Prior,
    int? AbsoluteChange,
    bool HasPriorPeriodComparison);

public record OnTimeKickoffValueDto(double? Rate, int Numerator, int Denominator, KpiPeriodStatus Status);

public record OnTimeKickoffKpiDto(
    OnTimeKickoffValueDto Current,
    OnTimeKickoffValueDto? Prior,
    double? AbsoluteChangeRate,
    bool HasPriorPeriodComparison);

public record KpiReportResponseDto(
    DateOnly? PeriodFrom,
    DateOnly? PeriodTo,
    DateOnly? PriorPeriodFrom,
    DateOnly? PriorPeriodTo,
    int RelevantEngagementCount,
    ReadinessTimeKpiDto ReadinessTime,
    AccessDelayKpiDto AccessDelay,
    EscalationCountKpiDto EscalationCount,
    OnTimeKickoffKpiDto OnTimeKickoffRate);
