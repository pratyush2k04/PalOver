namespace Pdr.Api.Exceptions;

public class UserNotAssignedToEngagementException(int engagementId)
    : AppException("Access denied. You're not assigned to this engagement.", StatusCodes.Status403Forbidden)
{
}
