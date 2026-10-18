using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class PersonNotFoundException(int personId, int engagementId)
    : AppException(EngagementValidationConstants.Messages.PersonNotFound(personId, engagementId), StatusCodes.Status404NotFound);
