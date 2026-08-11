using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class EngagementNotFoundException(int engagementId)
    : AppException(EngagementValidationConstants.Messages.NotFound(engagementId), StatusCodes.Status404NotFound);
