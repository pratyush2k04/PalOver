namespace Pdr.Api.Exceptions;

public class HandoverReadinessItemNotFoundException(int id)
    : AppException($"Handover readiness item with ID {id} was not found.", 404);

public class HandoverReadinessItemNameRequiredException()
    : AppException("Item name is required.", 400);

public class HandoverReadinessBlockReasonRequiredException()
    : AppException("Block reason is required when status is set to Blocked.", 400);

public class HandoverReadinessAccessDeniedException()
    : AppException("Only Pre-Sales can modify Handover Readiness items.", 403);
