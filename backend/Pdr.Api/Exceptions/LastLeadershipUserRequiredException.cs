using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class LastLeadershipUserRequiredException()
    : AppException(LeadershipAssignmentConstants.Messages.LastLeadershipUserRequired, StatusCodes.Status400BadRequest);
