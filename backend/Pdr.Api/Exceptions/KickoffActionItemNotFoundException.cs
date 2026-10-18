using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class KickoffActionItemNotFoundException(int actionItemId)
    : AppException($"Kickoff action item with ID {actionItemId} was not found.", StatusCodes.Status404NotFound);

public class KickoffActionItemStatusUpdateForbiddenException()
    : AppException("Only the assigned owner can update this action item's status.", StatusCodes.Status403Forbidden);
