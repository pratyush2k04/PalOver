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

public class ResourceRequestInvalidStateTransitionException(string message)
    : AppException(message, 400);

public class ResourceAllocationCountMismatchException()
    : AppException(ResourceRequestConstants.ValidationMessages.AllocationCountMismatch, 400);

public class ResourceAllocationNotFoundException(int id)
    : AppException(ResourceRequestConstants.ValidationMessages.AllocationNotFound(id), 404);

public class ResourceAllocationAlreadyConfirmedException()
    : AppException(ResourceRequestConstants.ValidationMessages.AllocationAlreadyConfirmed, 400);

public class ResourceAllocationNotConfirmedException()
    : AppException(ResourceRequestConstants.ValidationMessages.AllocationNotConfirmed, 400);

public class ResourceAllocationAccessDeniedException()
    : AppException(ResourceRequestConstants.ValidationMessages.AccessDeniedLeadershipOrAdmin, 403);
