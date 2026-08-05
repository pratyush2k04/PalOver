using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class DocumentAccessDeniedException(string message)
    : AppException(message, StatusCodes.Status403Forbidden);
