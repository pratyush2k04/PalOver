namespace Pdr.Api.Exceptions;

public class DiscoveryFeatureItemNotFoundException(int featureId)
    : AppException($"Discovery feature item with ID {featureId} was not found.", 404);
