using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class NotificationAccessDeniedException(int notificationId)
    : AppException(NotificationConstants.Messages.AccessDenied(notificationId), StatusCodes.Status403Forbidden);
