using System.Text.Json;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.DTOs.Kpi;
using Pdr.Api.DTOs.Notifications;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class KpiReportService(
    IEngagementRepository engagementRepository,
    IAuditLogRepository auditLogRepository,
    IAccessTrackerRepository accessTrackerRepository,
    INotificationRepository notificationRepository,
    IEngagementService engagementService,
    ICurrentUserService currentUserService) : IKpiReportService
{
    // TODO: replace with NotificationPriority.Escalation once the priority-4 escalation-notifications feature lands.
    private const int EscalationNotificationPriority = 4;

    private static readonly List<int> ReportAuditActionIds =
    [
        (int)AuditActionEnum.KickoffReadyMarked,
        (int)AuditActionEnum.AccessItemBlocked
    ];

    private record AccessItemBlockedPayload(int AccessItemId, int ActorUserId);
    private record EscalationEvent(int EngagementId, DateTime CreatedAt);

    public async Task<KpiReportResponseDto> GetReportAsync(KpiReportRequestDto request)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var period = KpiPeriodCalculator.Resolve(request.DatePreset, request.CustomFrom, request.CustomTo, today);

        var leadershipUserId = currentUserService.GetUserId();
        var scopedEngagementIds = await engagementRepository.GetEngagementIdsForUserAsync(leadershipUserId);

        if (scopedEngagementIds.Count == 0)
            return BuildEmptyResponse(period);

        // Sequential, not Task.WhenAll: these all share one scoped DbContext via their
        // repositories, and EF Core's DbContext does not support concurrent operations.
        var engagementInfos = await engagementRepository.GetEngagementReportInfoAsync(scopedEngagementIds);
        var assignments = await engagementRepository.GetAssignmentsForEngagementsAsync(scopedEngagementIds);
        var auditRows = await auditLogRepository.GetByEngagementIdsAndActionsAsync(scopedEngagementIds, ReportAuditActionIds);
        var resolvedAccessItems = await accessTrackerRepository.GetResolvedAccessItemsByEngagementIdsAsync(scopedEngagementIds);
        var priorityNotifications = await notificationRepository.GetByPriorityAsync(EscalationNotificationPriority);

        var relevantEngagementIds = ComputeRelevantEngagementIds(scopedEngagementIds, engagementInfos, assignments, request);

        if (relevantEngagementIds.Count == 0)
            return BuildEmptyResponse(period);

        var achievedDateByEngagementId = BuildAchievedDateLookup(auditRows);
        var blockedAtByAccessItemId = BuildBlockedAtLookup(auditRows);
        var escalationEvents = BuildEscalationEvents(priorityNotifications, relevantEngagementIds);

        var relevantEngagementInfos = engagementInfos.Where(e => relevantEngagementIds.Contains(e.EngagementId)).ToList();
        var relevantAccessItems = resolvedAccessItems.Where(a => relevantEngagementIds.Contains(a.EngagementId)).ToList();

        var readinessTime = BuildReadinessTimeKpi(relevantEngagementInfos, achievedDateByEngagementId, period);
        var accessDelay = BuildAccessDelayKpi(relevantAccessItems, blockedAtByAccessItemId, period);
        var escalationCount = BuildEscalationCountKpi(escalationEvents, period);
        var onTimeKickoff = BuildOnTimeKickoffKpi(relevantEngagementInfos, achievedDateByEngagementId, period, today);

        return new KpiReportResponseDto(
            period.From,
            period.To,
            period.PriorFrom,
            period.PriorTo,
            relevantEngagementIds.Count,
            readinessTime,
            accessDelay,
            escalationCount,
            onTimeKickoff);
    }

    public async Task<KpiReportFilterOptionsDto> GetFilterOptionsAsync()
    {
        var leadershipUserId = currentUserService.GetUserId();
        var scopedEngagementIds = await engagementRepository.GetEngagementIdsForUserAsync(leadershipUserId);

        if (scopedEngagementIds.Count == 0)
            return new KpiReportFilterOptionsDto([], []);

        var assignments = await engagementRepository.GetAssignmentsForEngagementsAsync(scopedEngagementIds);
        var allEngagementTypes = await engagementService.GetEngagementTypesAsync();

        var availablePms = assignments
            .Where(a => a.User.RoleId == (int)RoleEnum.ProjectManagement)
            .Select(a => new SimpleUserDto { Id = a.User.Id, Name = a.User.FullName })
            .DistinctBy(u => u.Id)
            .OrderBy(u => u.Name)
            .ToList();

        return new KpiReportFilterOptionsDto(availablePms, allEngagementTypes.ToList());
    }

    private static HashSet<int> ComputeRelevantEngagementIds(
        List<int> scopedEngagementIds,
        List<EngagementReportInfoDto> engagementInfos,
        List<Models.EngagementAssignment.EngagementAssignment> assignments,
        KpiReportRequestDto request)
    {
        var relevant = new HashSet<int>(scopedEngagementIds);

        if (request.PmUserIds is { Count: > 0 })
        {
            var pmEngagementIds = assignments
                .Where(a => a.User.RoleId == (int)RoleEnum.ProjectManagement && request.PmUserIds.Contains(a.UserId))
                .Select(a => a.EngagementId)
                .ToHashSet();
            relevant.IntersectWith(pmEngagementIds);
        }

        if (request.EngagementTypeIds is { Count: > 0 })
        {
            var typeEngagementIds = engagementInfos
                .Where(e => request.EngagementTypeIds.Contains(e.EngagementTypeId))
                .Select(e => e.EngagementId)
                .ToHashSet();
            relevant.IntersectWith(typeEngagementIds);
        }

        return relevant;
    }

    private static Dictionary<int, DateTime> BuildAchievedDateLookup(List<DTOs.Audit.AuditLogLiteDto> auditRows) =>
        auditRows
            .Where(r => r.AuditActionId == (int)AuditActionEnum.KickoffReadyMarked)
            .GroupBy(r => r.EngagementId)
            .ToDictionary(g => g.Key, g => g.Max(r => r.CreatedAt));

    private static Dictionary<int, DateTime> BuildBlockedAtLookup(List<DTOs.Audit.AuditLogLiteDto> auditRows)
    {
        var result = new Dictionary<int, DateTime>();

        foreach (var row in auditRows.Where(r => r.AuditActionId == (int)AuditActionEnum.AccessItemBlocked))
        {
            AccessItemBlockedPayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<AccessItemBlockedPayload>(row.PayloadJson);
            }
            catch (JsonException)
            {
                continue;
            }

            if (payload is null)
                continue;

            if (!result.TryGetValue(payload.AccessItemId, out var existing) || row.CreatedAt > existing)
                result[payload.AccessItemId] = row.CreatedAt;
        }

        return result;
    }

    private static List<EscalationEvent> BuildEscalationEvents(List<NotificationLiteDto> notifications, HashSet<int> relevantEngagementIds)
    {
        var events = new List<EscalationEvent>();

        foreach (var notification in notifications)
        {
            if (string.IsNullOrWhiteSpace(notification.Data))
                continue;

            int engagementId;
            try
            {
                using var doc = JsonDocument.Parse(notification.Data);
                if (!doc.RootElement.TryGetProperty("EngagementId", out var engagementIdElement))
                    continue;
                engagementId = engagementIdElement.GetInt32();
            }
            catch (JsonException)
            {
                continue;
            }

            if (relevantEngagementIds.Contains(engagementId))
                events.Add(new EscalationEvent(engagementId, notification.CreatedAt));
        }

        return events;
    }

    private static bool IsInWindow(DateOnly date, DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && date < from.Value) return false;
        if (to.HasValue && date > to.Value) return false;
        return true;
    }

    private static ReadinessTimeKpiDto BuildReadinessTimeKpi(
        List<EngagementReportInfoDto> engagements,
        Dictionary<int, DateTime> achievedDateByEngagementId,
        KpiPeriodRange period)
    {
        var current = CalculateReadinessTime(engagements, achievedDateByEngagementId, period.From, period.To);
        var prior = period.HasPrior
            ? CalculateReadinessTime(engagements, achievedDateByEngagementId, period.PriorFrom, period.PriorTo)
            : null;

        double? change = current.AverageDays.HasValue && prior?.AverageDays is not null
            ? current.AverageDays - prior.AverageDays
            : null;

        return new ReadinessTimeKpiDto(current, prior, change, period.HasPrior);
    }

    private static ReadinessTimeValueDto CalculateReadinessTime(
        List<EngagementReportInfoDto> engagements,
        Dictionary<int, DateTime> achievedDateByEngagementId,
        DateOnly? from,
        DateOnly? to)
    {
        var diffs = new List<double>();

        foreach (var engagement in engagements)
        {
            if (!IsInWindow(engagement.KickoffDate, from, to))
                continue;

            if (!achievedDateByEngagementId.TryGetValue(engagement.EngagementId, out var achievedAt))
                continue;

            var achievedDate = DateOnly.FromDateTime(achievedAt);
            diffs.Add(engagement.KickoffDate.DayNumber - achievedDate.DayNumber);
        }

        return diffs.Count == 0
            ? new ReadinessTimeValueDto(null, KpiPeriodStatus.NoEngagementsInPeriod)
            : new ReadinessTimeValueDto(diffs.Average(), KpiPeriodStatus.Ok);
    }

    private static AccessDelayKpiDto BuildAccessDelayKpi(
        List<AccessItemDelayInfoDto> accessItems,
        Dictionary<int, DateTime> blockedAtByAccessItemId,
        KpiPeriodRange period)
    {
        var current = CalculateAccessDelay(accessItems, blockedAtByAccessItemId, period.From, period.To);
        var prior = period.HasPrior
            ? CalculateAccessDelay(accessItems, blockedAtByAccessItemId, period.PriorFrom, period.PriorTo)
            : null;

        double? change = current.AverageDays.HasValue && prior?.AverageDays is not null
            ? current.AverageDays - prior.AverageDays
            : null;

        return new AccessDelayKpiDto(current, prior, change, period.HasPrior);
    }

    private static KpiAverageDaysValueDto CalculateAccessDelay(
        List<AccessItemDelayInfoDto> accessItems,
        Dictionary<int, DateTime> blockedAtByAccessItemId,
        DateOnly? from,
        DateOnly? to)
    {
        var delays = new List<double>();

        foreach (var item in accessItems)
        {
            DateOnly resolveDate;

            if (item.Status == AccessItemStatus.Granted && item.GrantedDate.HasValue)
            {
                resolveDate = item.GrantedDate.Value;
            }
            else if (item.Status == AccessItemStatus.Blocked && blockedAtByAccessItemId.TryGetValue(item.Id, out var blockedAt))
            {
                resolveDate = DateOnly.FromDateTime(blockedAt);
            }
            else
            {
                continue;
            }

            if (!IsInWindow(resolveDate, from, to))
                continue;

            var requestDate = DateOnly.FromDateTime(item.CreatedAt);
            delays.Add(resolveDate.DayNumber - requestDate.DayNumber);
        }

        return delays.Count == 0
            ? new KpiAverageDaysValueDto(null, KpiPeriodStatus.NoEngagementsInPeriod)
            : new KpiAverageDaysValueDto(delays.Average(), KpiPeriodStatus.Ok);
    }

    private static EscalationCountKpiDto BuildEscalationCountKpi(List<EscalationEvent> events, KpiPeriodRange period)
    {
        var current = CalculateEscalationCount(events, period.From, period.To);
        var prior = period.HasPrior
            ? CalculateEscalationCount(events, period.PriorFrom, period.PriorTo)
            : null;

        int? change = current.Count.HasValue && prior?.Count is not null
            ? current.Count - prior.Count
            : null;

        return new EscalationCountKpiDto(current, prior, change, period.HasPrior);
    }

    private static KpiCountValueDto CalculateEscalationCount(List<EscalationEvent> events, DateOnly? from, DateOnly? to)
    {
        var count = events.Count(e => IsInWindow(DateOnly.FromDateTime(e.CreatedAt), from, to));
        return new KpiCountValueDto(count, KpiPeriodStatus.Ok);
    }

    private static OnTimeKickoffKpiDto BuildOnTimeKickoffKpi(
        List<EngagementReportInfoDto> engagements,
        Dictionary<int, DateTime> achievedDateByEngagementId,
        KpiPeriodRange period,
        DateOnly today)
    {
        var current = CalculateOnTimeKickoffRate(engagements, achievedDateByEngagementId, period.From, period.To, today);
        var prior = period.HasPrior
            ? CalculateOnTimeKickoffRate(engagements, achievedDateByEngagementId, period.PriorFrom, period.PriorTo, today)
            : null;

        double? change = current.Rate.HasValue && prior?.Rate is not null
            ? current.Rate - prior.Rate
            : null;

        return new OnTimeKickoffKpiDto(current, prior, change, period.HasPrior);
    }

    private static OnTimeKickoffValueDto CalculateOnTimeKickoffRate(
        List<EngagementReportInfoDto> engagements,
        Dictionary<int, DateTime> achievedDateByEngagementId,
        DateOnly? from,
        DateOnly? to,
        DateOnly today)
    {
        var denominatorEngagements = engagements
            .Where(e => IsInWindow(e.KickoffDate, from, to) && e.KickoffDate <= today)
            .ToList();

        var denominator = denominatorEngagements.Count;
        var numerator = denominatorEngagements.Count(e =>
            achievedDateByEngagementId.TryGetValue(e.EngagementId, out var achievedAt) &&
            DateOnly.FromDateTime(achievedAt) <= e.KickoffDate);

        return denominator == 0
            ? new OnTimeKickoffValueDto(null, 0, 0, KpiPeriodStatus.NoEngagementsInPeriod)
            : new OnTimeKickoffValueDto((double)numerator / denominator, numerator, denominator, KpiPeriodStatus.Ok);
    }

    private static KpiReportResponseDto BuildEmptyResponse(KpiPeriodRange period)
    {
        var emptyReadiness = new ReadinessTimeValueDto(null, KpiPeriodStatus.NoEngagementsInPeriod);
        var emptyAccessDelay = new KpiAverageDaysValueDto(null, KpiPeriodStatus.NoEngagementsInPeriod);
        var emptyEscalation = new KpiCountValueDto(null, KpiPeriodStatus.NoEngagementsInPeriod);
        var emptyOnTime = new OnTimeKickoffValueDto(null, 0, 0, KpiPeriodStatus.NoEngagementsInPeriod);

        return new KpiReportResponseDto(
            period.From,
            period.To,
            period.PriorFrom,
            period.PriorTo,
            RelevantEngagementCount: 0,
            new ReadinessTimeKpiDto(emptyReadiness, null, null, false),
            new AccessDelayKpiDto(emptyAccessDelay, null, null, false),
            new EscalationCountKpiDto(emptyEscalation, null, null, false),
            new OnTimeKickoffKpiDto(emptyOnTime, null, null, false));
    }
}
