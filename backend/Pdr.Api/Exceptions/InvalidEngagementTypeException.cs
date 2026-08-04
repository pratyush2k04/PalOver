using Microsoft.AspNetCore.Http;
namespace Pdr.Api.Exceptions;

public class InvalidEngagementTypeException(int engagementTypeId)
    : AppException($"Engagement type with id {engagementTypeId} does not exist.", StatusCodes.Status400BadRequest);
