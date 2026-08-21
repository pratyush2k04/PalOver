using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class NotificationDataInvalidException(string templateKey)
    : AppException(NotificationConstants.Messages.InvalidPayloadData(templateKey), StatusCodes.Status400BadRequest);
