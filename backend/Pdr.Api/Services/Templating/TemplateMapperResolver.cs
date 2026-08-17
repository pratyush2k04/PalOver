namespace Pdr.Api.Services.Templating;

public class TemplateMapperResolver<TMapper>(IEnumerable<TMapper> mappers) where TMapper : ITemplateMapper
{
    private readonly Dictionary<string, TMapper> _mappersByKey =
        mappers.ToDictionary(m => m.TemplateKey);

    public TMapper? Resolve(string templateKey) =>
        _mappersByKey.GetValueOrDefault(templateKey);
}
