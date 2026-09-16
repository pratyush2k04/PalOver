using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class BlockerEscalationService(
    IRiskRepository riskRepository,
    IEngagementRepository engagementRepository,
    INotificationService notificationService,
    ILogger<BlockerEscalationService> logger) : IBlockerEscalationService
{
    public async Task<int> ProcessEscalationsAsync()
    {
        var now = DateTime.UtcNow;
        int escalationCount = 0;

        // Fetch all active open/mitigating risks along with engagement info
        var openRisks = await riskRepository.GetOpenRisksAsync();

        if (!openRisks.Any())
        {
            return 0;
        }

        var openRiskIds = openRisks.Select(r => r.Id).ToList();
        var allHistories = await riskRepository.GetEscalationHistoriesByRiskIdsAsync(openRiskIds);

        var latestEscalationPerRisk = allHistories
            .GroupBy(h => h.RiskId)
            .ToDictionary(g => g.Key, g => g.First());

        var engagementIds = openRisks.Select(r => r.EngagementId).Distinct().ToList();
        var engagements = await engagementRepository.GetByIdsAsync(engagementIds);

        foreach (var risk in openRisks)
        {
            if (!engagements.TryGetValue(risk.EngagementId, out var engagement))
            {
                continue;
            }

            latestEscalationPerRisk.TryGetValue(risk.Id, out var latestEscalation);
            var currentEscalationLevel = latestEscalation?.EscalationLevel;
            var lastEscalatedAt = latestEscalation?.EscalatedAt;

            var kickoffDateTime = engagement.KickoffDate.ToDateTime(TimeOnly.MinValue);
            var daysToKickoff = (kickoffDateTime.Date - now.Date).TotalDays;
            bool isAccelerated = daysToKickoff <= 3;

            var openWorkingDays = GetWorkingDays(risk.CreatedAt, now);
            var daysSinceLastEscalation = isAccelerated
                ? (int)(now - (lastEscalatedAt ?? risk.CreatedAt)).TotalDays
                : GetWorkingDays(lastEscalatedAt ?? risk.CreatedAt, now);

            if (isAccelerated)
            {
                if (daysSinceLastEscalation >= 1)
                {
                    var acceleratedReason = BlockerEscalationConstants.Messages.AcceleratedDailyEscalation((int)daysToKickoff);

                    // Accelerated Cadence: Notify BOTH PM and Leadership daily
                    await EscalateToLevelAsync(
                        risk,
                        engagement,
                        RiskEscalationLevel.Leadership,
                        RoleEnum.ProjectManagement,
                        NotificationConstants.TemplateKeys.BlockerEscalatedPM,
                        acceleratedReason,
                        openWorkingDays,
                        now);

                    await EscalateToLevelAsync(
                        risk,
                        engagement,
                        RiskEscalationLevel.Leadership,
                        RoleEnum.Leadership,
                        NotificationConstants.TemplateKeys.BlockerEscalatedLeadership,
                        acceleratedReason,
                        openWorkingDays,
                        now);

                    escalationCount++;
                }
            }
            else
            {
                // Normal Cadence: > 2 working days -> PM, then +2 working days -> Leadership
                if (currentEscalationLevel == null && openWorkingDays > 2)
                {
                    await EscalateToLevelAsync(
                        risk,
                        engagement,
                        RiskEscalationLevel.PM,
                        RoleEnum.ProjectManagement,
                        NotificationConstants.TemplateKeys.BlockerEscalatedPM,
                        BlockerEscalationConstants.Messages.UnresolvedPm(openWorkingDays),
                        openWorkingDays,
                        now);

                    escalationCount++;
                }
                else if (currentEscalationLevel == RiskEscalationLevel.PM && daysSinceLastEscalation >= 2)
                {
                    await EscalateToLevelAsync(
                        risk,
                        engagement,
                        RiskEscalationLevel.Leadership,
                        RoleEnum.Leadership,
                        NotificationConstants.TemplateKeys.BlockerEscalatedLeadership,
                        BlockerEscalationConstants.Messages.UnresolvedLeadership(daysSinceLastEscalation),
                        openWorkingDays,
                        now);

                    escalationCount++;
                }
            }
        }

        if (escalationCount > 0)
        {
            await riskRepository.SaveChangesAsync();
            logger.LogInformation("Processed {Count} blocker escalations at {Timestamp}.", escalationCount, now);
        }

        return escalationCount;
    }

    public async Task<List<RiskEscalationHistoryDto>> GetEscalationHistoryAsync(int riskId)
    {
        var history = await riskRepository.GetEscalationHistoryByRiskIdAsync(riskId);

        return history.Select(h => new RiskEscalationHistoryDto
        {
            Id = h.Id,
            RiskId = h.RiskId,
            EscalatedAt = h.EscalatedAt,
            EscalationLevel = h.EscalationLevel.ToString(),
            EscalatedToUserId = h.EscalatedToUserId,
            EscalatedToUserName = h.EscalatedToUser?.FullName,
            EscalatedToRole = h.EscalatedToRole,
            Reason = h.Reason
        }).ToList();
    }

    private async Task EscalateToLevelAsync(
        Risk risk,
        Engagement engagement,
        RiskEscalationLevel targetLevel,
        RoleEnum targetRole,
        string templateKey,
        string reason,
        int unresolvedDays,
        DateTime timestamp)
    {
        var targetEngagementRoleId = targetRole switch
        {
            RoleEnum.ProjectManagement => (int)EngagementRoleEnum.ProjectManager,
            RoleEnum.Leadership => (int)EngagementRoleEnum.Leadership,
            _ => (int)targetRole
        };

        var pmOrLeadershipAssignment = await engagementRepository.GetAssignmentByEngagementRoleIdAsync(engagement.EngagementId, targetEngagementRoleId);

        var historyRecord = new RiskEscalationHistory
        {
            RiskId = risk.Id,
            EscalatedAt = timestamp,
            EscalationLevel = targetLevel,
            EscalatedToUserId = pmOrLeadershipAssignment?.UserId,
            EscalatedToRole = targetRole.ToString(),
            Reason = reason
        };

        await riskRepository.AddEscalationHistoryAsync(historyRecord);

        var dataJson = JsonSerializer.Serialize(new
        {
            EngagementId = engagement.EngagementId,
            RiskId = risk.Id,
            ItemTitle = risk.Title,
            UnresolvedDays = unresolvedDays
        });

        var link = BlockerEscalationConstants.Routes.RisksWorkspaceLink(engagement.EngagementId);

        if (targetRole == RoleEnum.Leadership)
        {
            var leadershipAssignments = await engagementRepository.GetLeadershipAssignmentsAsync(engagement.EngagementId);
            var leadershipUserIds = leadershipAssignments.Select(a => a.UserId).Distinct().ToList();

            if (leadershipUserIds.Any())
            {
                await notificationService.CreateFromTemplateAsync(
                    templateKey,
                    leadershipUserIds,
                    dataJson,
                    timestamp);
            }
        }
        else
        {
            await notificationService.CreateFromTemplateAsync(
                templateKey,
                engagement.EngagementId,
                targetRole,
                dataJson,
                timestamp);
        }
    }

    public static int GetWorkingDays(DateTime fromDate, DateTime toDate)
    {
        if (fromDate >= toDate) return 0;

        int workingDays = 0;
        for (var date = fromDate.Date.AddDays(1); date <= toDate.Date; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
        }

        return workingDays;
    }
}
