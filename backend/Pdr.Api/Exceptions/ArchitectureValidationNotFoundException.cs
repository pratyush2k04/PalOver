namespace Pdr.Api.Exceptions;

public class ArchitectureValidationNotFoundException(int engagementId)
    : AppException($"Architecture validation for engagement {engagementId} was not found.", 404);
