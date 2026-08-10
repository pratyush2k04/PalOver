using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverNotFoundException(int engagementId)
    : AppException(HandoverValidationConstants.NotFoundForEngagement(engagementId), StatusCodes.Status404NotFound);
