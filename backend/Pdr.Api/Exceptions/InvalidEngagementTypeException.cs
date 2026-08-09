using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class InvalidEngagementTypeException(int engagementTypeId)
    : AppException(EngagementValidationConstants.Messages.InvalidType(engagementTypeId), StatusCodes.Status400BadRequest);
