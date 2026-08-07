namespace Pdr.Api.Exceptions;

public class ChangeRequestNotFoundException(int id) 
    : AppException($"Change request with ID {id} was not found.", 404);

public class ChangeRequestSelfApprovalException() 
    : AppException("The user who raised the change request cannot approve or reject it.", 403);

public class ChangeRequestInvalidStateTransitionException(string message) 
    : AppException(message, 400);

public class RejectionReasonRequiredException() 
    : AppException("Reason is required when rejecting a change request.", 400);
