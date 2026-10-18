using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class AccessItemNotFoundException(int accessItemId)
    : AppException(AccessTrackerConstants.Messages.NotFound(accessItemId), StatusCodes.Status404NotFound);
