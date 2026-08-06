using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions.AccessItemExceptions;

public class BlockReasonRequiredException()
    : AppException("A block reason must be provided when setting the status to Blocked.", StatusCodes.Status400BadRequest);
