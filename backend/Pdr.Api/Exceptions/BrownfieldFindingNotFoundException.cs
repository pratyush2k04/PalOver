namespace Pdr.Api.Exceptions;

public class BrownfieldFindingNotFoundException(int id)
    : AppException($"Brownfield finding with ID {id} was not found.", 404);
