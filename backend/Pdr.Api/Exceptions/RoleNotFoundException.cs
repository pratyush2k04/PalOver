using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class RoleNotFoundException(int roleId)
    : AppException(AuthConstants.Messages.RoleNotFound(roleId), StatusCodes.Status400BadRequest);
