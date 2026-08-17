namespace Pdr.Api.Services.Templating;

public interface ITemplateMapper
{
    string TemplateKey { get; }

    Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default);
}
