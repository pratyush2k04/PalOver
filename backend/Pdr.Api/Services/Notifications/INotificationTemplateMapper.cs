namespace Pdr.Api.Services.Notifications;

public interface INotificationTemplateMapper
{
    string TemplateKey { get; }

    Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default);
}
