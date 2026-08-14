using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class KickoffActionItemNotFoundException(int actionItemId)
    : AppException($"Kickoff action item with ID {actionItemId} was not found.", StatusCodes.Status404NotFound);
