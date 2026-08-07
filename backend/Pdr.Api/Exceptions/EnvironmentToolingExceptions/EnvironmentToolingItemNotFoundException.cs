using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions.EnvironmentToolingExceptions;

public class EnvironmentToolingItemNotFoundException(int itemId)
    : AppException($"Environment/tooling item with id {itemId} was not found.", StatusCodes.Status404NotFound);
