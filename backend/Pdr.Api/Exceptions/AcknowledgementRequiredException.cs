using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class AcknowledgementRequiredException(string message)
    : AppException(message, StatusCodes.Status400BadRequest);
