using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class AssumptionNotFoundException(int assumptionId) 
    : AppException($"Assumption with ID {assumptionId} was not found.", StatusCodes.Status404NotFound)
{
}

public class AssumptionAlreadyValidatedException(int assumptionId)
    : AppException($"Assumption with ID {assumptionId} has already been validated and cannot be edited.", StatusCodes.Status400BadRequest)
{
}

public class InvalidAssumptionStatusException()
    : AppException("Cannot validate to 'Unvalidated' status.", StatusCodes.Status400BadRequest)
{
}

public class AssumptionCorrectionReasonMissingException()
    : AppException("A reason must be provided when correcting a validated or invalid assumption.", StatusCodes.Status400BadRequest)
{
}

public class AssumptionUnauthorizedException()
    : AppException("You are not authorized to modify this assumption.", StatusCodes.Status403Forbidden)
{
}
