using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class ResourceRequestAfterBillingDateException()
    : AppException(ResourceRequestConstants.ValidationMessages.AfterBillingDate, 400);

public class EngagementBillingDateNotSetException()
    : AppException(ResourceRequestConstants.ValidationMessages.BillingDateNotSet, 400);

public class ResourceRequestAccessDeniedException()
    : AppException(ResourceRequestConstants.ValidationMessages.AccessDenied, 403);

public class ResourceRequestNotFoundException(int id)
    : AppException(ResourceRequestConstants.ValidationMessages.NotFound(id), 404);
