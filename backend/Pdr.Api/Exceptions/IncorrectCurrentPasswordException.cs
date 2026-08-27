using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class IncorrectCurrentPasswordException()
    : AppException("Your current password is incorrect.", StatusCodes.Status400BadRequest);
