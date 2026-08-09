using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Exceptions;

public class HandoverNotFoundException(int engagementId)
    : AppException(HandoverValidationConstants.NotFoundForEngagement(engagementId), StatusCodes.Status404NotFound);
