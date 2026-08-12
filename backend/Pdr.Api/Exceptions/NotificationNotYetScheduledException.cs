using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class NotificationNotYetScheduledException(int notificationId)
    : AppException(NotificationConstants.Messages.NotYetScheduled(notificationId), StatusCodes.Status409Conflict);
