using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions.EnvironmentToolingExceptions;

public class NotApplicableReasonRequiredException()
    : AppException("NotApplicableReason is required when setting status to NotApplicable.", StatusCodes.Status400BadRequest);
