using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class NotificationTemplateNotFoundException(string templateKey)
    : AppException(NotificationConstants.Messages.TemplateNotFound(templateKey), StatusCodes.Status404NotFound);
