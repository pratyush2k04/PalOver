using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions.RiskExceptions;

public class RiskNotFoundException(int riskId)
    : AppException($"Risk with id {riskId} was not found.", StatusCodes.Status404NotFound);
