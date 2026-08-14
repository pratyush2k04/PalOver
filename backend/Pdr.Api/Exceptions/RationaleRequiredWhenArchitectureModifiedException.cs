using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class RationaleRequiredWhenArchitectureModifiedException()
    : AppException(ArchitectureValidationConstants.Messages.RationaleRequiredWhenModified, 400);
