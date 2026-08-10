using Microsoft.AspNetCore.Http;

using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions.KickoffReadinessExceptions;

public class KickoffNotReadyException()
    : AppException(KickoffReadinessConstants.Messages.EngagementNotFullyReady, StatusCodes.Status422UnprocessableEntity);
