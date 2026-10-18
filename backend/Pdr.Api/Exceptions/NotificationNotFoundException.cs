using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class NotificationNotFoundException(int notificationId)
    : AppException(NotificationConstants.Messages.NotFound(notificationId), StatusCodes.Status404NotFound);
