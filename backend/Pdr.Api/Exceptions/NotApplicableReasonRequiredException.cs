using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;
using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Exceptions;

public class NotApplicableReasonRequiredException()
    : AppException(EnvironmentToolingConstants.Messages.NotApplicableReasonRequired, StatusCodes.Status400BadRequest);
