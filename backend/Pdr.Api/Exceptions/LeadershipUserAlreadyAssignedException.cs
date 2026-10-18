using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class LeadershipUserAlreadyAssignedException()
    : AppException(LeadershipAssignmentConstants.Messages.LeadershipUserAlreadyAssigned, StatusCodes.Status400BadRequest);
