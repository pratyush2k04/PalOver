using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueryAlreadyRespondedException()
    : AppException(HandoverQueryConstants.AlreadyResponded, StatusCodes.Status409Conflict);
