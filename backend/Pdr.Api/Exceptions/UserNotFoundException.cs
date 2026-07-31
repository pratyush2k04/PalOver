using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class UserNotFoundException(int userId)
    : AppException($"User with id {userId} was not found.", StatusCodes.Status404NotFound);
