using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class InvalidOwnerRoleException(int userId, string roleName)
    : AppException(EngagementValidationConstants.Messages.InvalidOwnerRole(userId, roleName), StatusCodes.Status400BadRequest);
