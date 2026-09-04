using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueryAlreadyResolvedException()
    : AppException(HandoverQueryConstants.QueryAlreadyResolved, StatusCodes.Status409Conflict);
