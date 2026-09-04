using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class HandoverQueryInvalidTurnException(string message)
    : AppException(message, StatusCodes.Status409Conflict);
