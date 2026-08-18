namespace Pdr.Api.Exceptions;

public class UserNotAssignedToEngagementException(int engagementId, int userId)
    : AppException($"User {userId} is not assigned to engagement {engagementId}", StatusCodes.Status403Forbidden)
{
}
