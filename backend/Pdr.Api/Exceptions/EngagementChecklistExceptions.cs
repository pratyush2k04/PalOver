using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class EngagementChecklistItemNotFoundException(int itemId)
    : AppException($"Checklist item with ID {itemId} was not found.", StatusCodes.Status404NotFound);

public class ChecklistNotApplicableReasonRequiredException()
    : AppException("A reason is required when marking a checklist item as Not Applicable.", StatusCodes.Status400BadRequest);

public class InvalidChecklistItemStatusException(string status)
    : AppException($"Invalid checklist item status '{status}'. Status must be 'Complete' or 'Incomplete'.", StatusCodes.Status400BadRequest);

public class CannotUpdateNonApplicableItemException(int itemId)
    : AppException($"Cannot change status of non-applicable checklist item {itemId}.", StatusCodes.Status422UnprocessableEntity);

public class ExemptionNotPendingException(int itemId)
    : AppException($"Checklist item {itemId} does not have a pending exemption request.", StatusCodes.Status400BadRequest);
