namespace Pdr.Api.Services.Templating;

public interface ITemplateMapper
{
    string TemplateKey { get; }

    void CollectIds(string data, BatchLoadContext context) { }

    IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
        => new Dictionary<string, object?>();

    Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
        => Task.FromResult(Map(data, new BatchLoadContext()));
}
