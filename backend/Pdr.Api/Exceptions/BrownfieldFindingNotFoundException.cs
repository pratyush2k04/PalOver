using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class BrownfieldFindingNotFoundException(int id)
    : AppException(BrownfieldFindingConstants.Messages.NotFound(id), 404);
