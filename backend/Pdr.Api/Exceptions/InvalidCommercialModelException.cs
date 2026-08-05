using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class InvalidCommercialModelException(int commercialModelId)
    : AppException($"Commercial model with id {commercialModelId} does not exist.", StatusCodes.Status400BadRequest);
