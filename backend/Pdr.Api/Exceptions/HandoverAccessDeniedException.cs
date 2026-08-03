using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class HandoverAccessDeniedException(string message)
    : AppException(message, StatusCodes.Status403Forbidden);
