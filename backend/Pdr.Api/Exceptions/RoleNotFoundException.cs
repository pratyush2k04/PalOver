using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class RoleNotFoundException(int roleId)
    : AppException($"Role with id {roleId} was not found.", StatusCodes.Status400BadRequest);
