using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;
using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.Exceptions;

public class InvalidStatusTransitionException(AccessItemStatus from, AccessItemStatus to)
    : AppException(AccessTrackerConstants.Messages.InvalidStatusTransition(from.ToString(), to.ToString()), StatusCodes.Status422UnprocessableEntity);
