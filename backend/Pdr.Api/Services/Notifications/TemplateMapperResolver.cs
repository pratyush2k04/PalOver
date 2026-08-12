namespace Pdr.Api.Services.Notifications;

public class TemplateMapperResolver(IEnumerable<INotificationTemplateMapper> mappers)
{
    private readonly Dictionary<string, INotificationTemplateMapper> _mappersByKey =
        mappers.ToDictionary(m => m.TemplateKey);

    public INotificationTemplateMapper? Resolve(string templateKey) =>
        _mappersByKey.GetValueOrDefault(templateKey);
}
