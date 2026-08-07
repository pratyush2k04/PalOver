using Microsoft.AspNetCore.Http;

using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions.EnvironmentToolingExceptions;

public class NotApplicableReasonRequiredException()
    : AppException(EnvironmentToolingConstants.Messages.NotApplicableReasonRequired, StatusCodes.Status400BadRequest);
