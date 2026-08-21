using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class DependencySpikeNotFoundException(int id)
    : AppException(DependencySpikeConstants.Messages.NotFound(id), 404);
