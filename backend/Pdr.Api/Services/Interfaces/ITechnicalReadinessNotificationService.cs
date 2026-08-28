namespace Pdr.Api.Services.Interfaces;

public interface ITechnicalReadinessNotificationService
{
    Task CheckAndNotifyIfReadyAsync(int engagementId, bool wasReadyBeforeUpdate);
    Task<bool> IsReadyAsync(int engagementId);
}
