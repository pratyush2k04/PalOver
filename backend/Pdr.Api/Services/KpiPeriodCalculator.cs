using Pdr.Api.DTOs.Kpi;

namespace Pdr.Api.Services;

public record KpiPeriodRange(DateOnly? From, DateOnly? To, DateOnly? PriorFrom, DateOnly? PriorTo, bool HasPrior);

public static class KpiPeriodCalculator
{
    public static KpiPeriodRange Resolve(KpiReportDatePreset preset, DateOnly? customFrom, DateOnly? customTo, DateOnly today)
    {
        return preset switch
        {
            KpiReportDatePreset.AllTime => new KpiPeriodRange(null, null, null, null, HasPrior: false),
            KpiReportDatePreset.LastMonth => FromLength(today.AddMonths(-1), today),
            KpiReportDatePreset.Last6Months => FromLength(today.AddMonths(-6), today),
            KpiReportDatePreset.LastYear => FromLength(today.AddYears(-1), today),
            KpiReportDatePreset.Custom => FromCustom(customFrom, customTo),
            _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Unrecognized KPI report date preset.")
        };
    }

    private static KpiPeriodRange FromLength(DateOnly from, DateOnly to)
    {
        var lengthInDays = to.DayNumber - from.DayNumber;
        var priorTo = from;
        var priorFrom = from.AddDays(-lengthInDays);
        return new KpiPeriodRange(from, to, priorFrom, priorTo, HasPrior: true);
    }

    private static KpiPeriodRange FromCustom(DateOnly? customFrom, DateOnly? customTo)
    {
        var from = customFrom!.Value;
        var to = customTo!.Value;
        var lengthInDays = to.DayNumber - from.DayNumber;
        var priorTo = from;
        var priorFrom = from.AddDays(-lengthInDays);
        return new KpiPeriodRange(from, to, priorFrom, priorTo, HasPrior: true);
    }
}
