using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class ClientContactTransitionNotFoundException(int engagementId)
    : AppException(ClientContactTransitionValidationConstants.Messages.NotFound(engagementId), StatusCodes.Status404NotFound);
