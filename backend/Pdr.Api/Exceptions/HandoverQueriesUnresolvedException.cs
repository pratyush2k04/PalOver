using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class HandoverQueriesUnresolvedException()
    : AppException(HandoverValidationConstants.QueriesUnresolvedForReview, StatusCodes.Status409Conflict);
