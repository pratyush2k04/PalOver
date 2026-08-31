using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueryNotYetRespondedException()
    : AppException(HandoverQueryConstants.NotYetResponded, StatusCodes.Status409Conflict);
