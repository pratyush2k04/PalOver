namespace Pdr.Api.Exceptions;

public class UserNotAssignedToEngagementException(int engagementId)
    : AppException($"Access denied. You're not assigned to engagement {engagementId}.", StatusCodes.Status403Forbidden)
{
}
