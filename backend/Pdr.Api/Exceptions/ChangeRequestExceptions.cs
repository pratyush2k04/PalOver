using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class ChangeRequestNotFoundException(int id) 
    : AppException(ChangeRequestConstants.ValidationMessages.NotFound(id), 404);

public class ChangeRequestSelfApprovalException() 
    : AppException(ChangeRequestConstants.ValidationMessages.SelfApprovalNotAllowed, 403);

public class ChangeRequestInvalidStateTransitionException(string message) 
    : AppException(message, 400);

public class RejectionReasonRequiredException() 
    : AppException(ChangeRequestConstants.ValidationMessages.RejectionReasonRequired, 400);
