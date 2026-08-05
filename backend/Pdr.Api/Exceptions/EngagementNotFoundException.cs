using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class EngagementNotFoundException(int engagementId)
    : AppException($"Engagement with id {engagementId} was not found.", StatusCodes.Status404NotFound);
