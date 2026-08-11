using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class EngagementChecklistItemNotFoundException(int itemId)
    : AppException(MasterChecklistConstants.Messages.ChecklistItemNotFound(itemId), StatusCodes.Status404NotFound);

public class ChecklistNotApplicableReasonRequiredException()
    : AppException(MasterChecklistConstants.Messages.NotApplicableReasonRequired, StatusCodes.Status400BadRequest);

public class InvalidChecklistItemStatusException(string status)
    : AppException(MasterChecklistConstants.Messages.InvalidItemStatus(status), StatusCodes.Status400BadRequest);

public class CannotUpdateNonApplicableItemException(int itemId)
    : AppException(MasterChecklistConstants.Messages.CannotUpdateNonApplicableItem(itemId), StatusCodes.Status422UnprocessableEntity);

public class ExemptionNotPendingException(int itemId)
    : AppException(MasterChecklistConstants.Messages.ExemptionNotPending(itemId), StatusCodes.Status400BadRequest);

public class ChecklistBlockReasonRequiredException()
    : AppException(MasterChecklistConstants.Messages.BlockReasonRequired, StatusCodes.Status400BadRequest);

public class CannotUpdateBlockedItemException(int itemId)
    : AppException(MasterChecklistConstants.Messages.CannotUpdateBlockedItem(itemId), StatusCodes.Status422UnprocessableEntity);

public class ItemNotBlockedException(int itemId)
    : AppException(MasterChecklistConstants.Messages.ItemNotBlocked(itemId), StatusCodes.Status400BadRequest);
