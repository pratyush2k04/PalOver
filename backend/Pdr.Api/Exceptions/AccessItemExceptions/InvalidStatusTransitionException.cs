using Microsoft.AspNetCore.Http;
using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.Exceptions.AccessItemExceptions;

public class InvalidStatusTransitionException(AccessItemStatus from, AccessItemStatus to)
    : AppException($"Cannot transition access item from '{from}' to '{to}'.", StatusCodes.Status422UnprocessableEntity);
