using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class KickoffRecordNotFoundException(int engagementId)
    : AppException($"Kickoff meeting record for Engagement {engagementId} was not found.", StatusCodes.Status404NotFound);
