using Pdr.Api.Models.Engagements.Dependencies;

namespace Pdr.Api.Exceptions;

public class DependencyNotFoundException : AppException
{
    public DependencyNotFoundException(int dependencyId) 
        : base($"Dependency with ID {dependencyId} was not found.", 404)
    {
    }
}

public class DependencyValidationException : AppException
{
    public DependencyValidationException(string message) 
        : base(message, 400)
    {
    }
}
