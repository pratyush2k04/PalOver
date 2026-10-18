using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class InvalidRoleMappingException(string userRole, string engagementRole)
    : AppException(EngagementValidationConstants.Messages.InvalidRoleMapping(userRole, engagementRole), StatusCodes.Status400BadRequest);
