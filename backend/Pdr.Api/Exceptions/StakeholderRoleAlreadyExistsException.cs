using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class StakeholderRoleAlreadyExistsException(string role)
    : AppException(CadenceConstants.Messages.StakeholderRoleAlreadyExists(role), StatusCodes.Status400BadRequest);
