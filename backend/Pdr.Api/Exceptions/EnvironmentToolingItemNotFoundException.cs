using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class EnvironmentToolingItemNotFoundException(int itemId)
    : AppException(EnvironmentToolingConstants.Messages.NotFound(itemId), StatusCodes.Status404NotFound);
