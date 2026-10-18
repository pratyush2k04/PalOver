using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class EngagementNameRequiredException()
    : AppException(EngagementValidationConstants.Messages.EngagementNameRequired, StatusCodes.Status400BadRequest);
