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
        if (wasReadyBeforeUpdate)
        {
            return;
        }

        var isNowReady = await IsReadyAsync(engagementId);
        if (isNowReady)
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
    }
}
