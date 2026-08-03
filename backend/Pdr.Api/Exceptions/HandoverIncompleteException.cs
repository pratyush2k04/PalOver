using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class HandoverIncompleteException(string message)
    : AppException(message, StatusCodes.Status400BadRequest);
