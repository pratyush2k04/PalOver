using Microsoft.AspNetCore.Http;
using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Exceptions.EnvironmentToolingExceptions;

public class InvalidEnvironmentToolingStatusTransitionException(EnvironmentToolingStatus fromStatus, EnvironmentToolingStatus toStatus)
    : AppException($"Cannot transition status from {fromStatus} to {toStatus}.", StatusCodes.Status422UnprocessableEntity);
