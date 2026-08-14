using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverNotYetSubmittedException()
    : AppException(HandoverValidationConstants.NotYetSubmitted, StatusCodes.Status400BadRequest);
