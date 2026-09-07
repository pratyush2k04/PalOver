namespace Pdr.Api.Services.Templating;

public interface IBatchDataPreloader
{
    Task PreloadAsync(BatchLoadContext context, CancellationToken cancellationToken = default);
}
