using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class TechnicalReadinessNotificationService(
    ITechnicalSummaryService technicalSummaryService,
    INotificationService notificationService,
    ILogger<TechnicalReadinessNotificationService> logger) : ITechnicalReadinessNotificationService
{
    public async Task<bool> IsReadyAsync(int engagementId)
    {
        try
        {
            var summary = await technicalSummaryService.GetDayOneSummaryAsync(engagementId);
            return summary.IsAbleToStartDayOne;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to evaluate technical readiness for engagement {EngagementId}", engagementId);
            return false;
        }
    }

    public async Task CheckAndNotifyIfReadyAsync(int engagementId, bool wasReadyBeforeUpdate)
    {
        try
        {
            var summary = await technicalSummaryService.GetDayOneSummaryAsync(engagementId);
            var isNowReady = summary.IsAbleToStartDayOne;

            if (!wasReadyBeforeUpdate && isNowReady)
            {
                logger.LogInformation("Technical readiness reached 100% for engagement {EngagementId}. Triggering notification to PM.", engagementId);

                var payload = JsonSerializer.Serialize(
                    new TechnicalReadinessCompleteMapper.TechnicalReadinessCompletePayload(engagementId));

                await notificationService.CreateFromTemplateAsync(
                    NotificationConstants.TemplateKeys.TechnicalReadinessComplete,
                    engagementId,
                    EngagementRoleEnum.ProjectManager,
                    payload,
                    DateTime.UtcNow);
            }
            else if (wasReadyBeforeUpdate && !isNowReady)
            {
                logger.LogInformation("Technical readiness dropped below 100% for engagement {EngagementId}. Triggering regression notification to PM.", engagementId);

                var reasons = new List<string>();
                if (!summary.IsAccessReady) reasons.Add("Access items incomplete or blocked");
                if (!summary.IsToolingReady) reasons.Add("Environment tooling incomplete or blocked");
                if (!summary.IsArchitectureReady) reasons.Add("Discovery scope estimate exceeds committed timeline");
                if (!summary.IsBrownfieldReady) reasons.Add("Brownfield high-severity finding pending client decision");
                if (reasons.Count == 0 && summary.OutstandingBlockers.Count > 0)
                {
                    reasons.AddRange(summary.OutstandingBlockers.Select(b => $"{b.Category}: {b.Name}"));
                }

                var reasonText = reasons.Count > 0 ? string.Join("; ", reasons) : "unresolved technical blockers";

                var payload = JsonSerializer.Serialize(
                    new TechnicalReadinessRegressedMapper.TechnicalReadinessRegressedPayload(engagementId, reasonText));

                await notificationService.CreateFromTemplateAsync(
                    NotificationConstants.TemplateKeys.TechnicalReadinessRegressed,
                    engagementId,
                    EngagementRoleEnum.ProjectManager,
                    payload,
                    DateTime.UtcNow);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to evaluate technical readiness notification check for engagement {EngagementId}", engagementId);
        }
    }
}
