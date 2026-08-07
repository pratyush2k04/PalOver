using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class EstimationGapAcknowledgementRequiredException(string message)
    : AppException(message, StatusCodes.Status400BadRequest);
