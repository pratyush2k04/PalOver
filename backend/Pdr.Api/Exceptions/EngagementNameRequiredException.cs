using Microsoft.AspNetCore.Http;
namespace Pdr.Api.Exceptions;

public class EngagementNameRequiredException()
: AppException("Engagement name is required.", StatusCodes.Status400BadRequest);
