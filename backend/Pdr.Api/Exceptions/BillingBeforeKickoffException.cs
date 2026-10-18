using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class BillingBeforeKickoffException() 
    : AppException(EngagementValidationConstants.Messages.BillingBeforeKickoff, StatusCodes.Status400BadRequest);
