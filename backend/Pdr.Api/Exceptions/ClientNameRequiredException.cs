using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class ClientNameRequiredException()
    : AppException(EngagementValidationConstants.Messages.ClientNameRequired, StatusCodes.Status400BadRequest);
