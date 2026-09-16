using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class EngagementRoleAccessDeniedException(string message)
    : AppException(message, StatusCodes.Status403Forbidden);
