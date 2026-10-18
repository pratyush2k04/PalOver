using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class DependencyNotFoundException(int dependencyId) 
    : AppException(DependencyConstants.Messages.DependencyNotFound(dependencyId), StatusCodes.Status404NotFound);

public class DependencyUnauthorizedException() 
    : AppException(DependencyConstants.Messages.Unauthorized, StatusCodes.Status403Forbidden);


