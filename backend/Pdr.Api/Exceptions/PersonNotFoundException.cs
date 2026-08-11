using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class PersonNotFoundException(int personId, int engagementId)
    : AppException($"Person with ID {personId} for Engagement {engagementId} was not found.", StatusCodes.Status404NotFound);
