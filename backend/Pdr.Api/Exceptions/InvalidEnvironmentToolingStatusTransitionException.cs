using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;
using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Exceptions;

public class InvalidEnvironmentToolingStatusTransitionException(EnvironmentToolingStatus fromStatus, EnvironmentToolingStatus toStatus)
    : AppException(EnvironmentToolingConstants.Messages.InvalidStatusTransition(fromStatus.ToString(), toStatus.ToString()), StatusCodes.Status422UnprocessableEntity);
