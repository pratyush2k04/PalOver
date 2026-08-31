using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueryNotFoundException(int queryId)
    : AppException(HandoverQueryConstants.NotFound(queryId), StatusCodes.Status404NotFound);
