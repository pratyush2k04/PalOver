using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class InvalidCommercialModelException(int commercialModelId)
    : AppException(EngagementValidationConstants.Messages.InvalidCommercialModel(commercialModelId), StatusCodes.Status400BadRequest);
