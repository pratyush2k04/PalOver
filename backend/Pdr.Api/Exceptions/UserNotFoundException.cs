using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class UserNotFoundException(int userId)
    : AppException(AuthConstants.Messages.UserNotFound(userId), StatusCodes.Status404NotFound);
