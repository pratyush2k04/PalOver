using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class HandoverNotFoundException(long engagementId)
    : AppException($"No handover draft exists yet for engagement {engagementId}. Save the Client Context section first.", StatusCodes.Status404NotFound);
