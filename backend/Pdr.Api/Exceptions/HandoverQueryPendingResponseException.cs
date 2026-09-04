using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueryPendingResponseException()
    : AppException(HandoverQueryConstants.CannotResolvePendingQuestion, StatusCodes.Status409Conflict);
