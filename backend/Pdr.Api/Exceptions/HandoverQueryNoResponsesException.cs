using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueryNoResponsesException()
    : AppException(HandoverQueryConstants.CannotResolveWithoutResponse, StatusCodes.Status409Conflict);
