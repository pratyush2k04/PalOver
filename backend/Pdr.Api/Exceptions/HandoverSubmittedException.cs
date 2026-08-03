using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class HandoverSubmittedException(string message)
    : AppException(message, StatusCodes.Status400BadRequest);
