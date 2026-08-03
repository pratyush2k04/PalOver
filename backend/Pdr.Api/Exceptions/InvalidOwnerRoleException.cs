using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class InvalidOwnerRoleException(int userId, string requiredRole)
    : AppException($"User with id {userId} does not hold the {requiredRole} role.", StatusCodes.Status400BadRequest);
