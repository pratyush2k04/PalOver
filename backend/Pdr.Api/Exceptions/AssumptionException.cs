using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class AssumptionNotFoundException(int assumptionId) 
    : AppException(AssumptionConstants.Messages.AssumptionNotFound(assumptionId), StatusCodes.Status404NotFound);


public class AssumptionAlreadyValidatedException(int assumptionId)
    : AppException(AssumptionConstants.Messages.AlreadyValidated(assumptionId), StatusCodes.Status400BadRequest);


public class InvalidAssumptionStatusException()
    : AppException(AssumptionConstants.Messages.InvalidValidationStatus, StatusCodes.Status400BadRequest);

public class AssumptionUnauthorizedException()
    : AppException(AssumptionConstants.Messages.Unauthorized, StatusCodes.Status403Forbidden);

