using Microsoft.AspNetCore.Http;
namespace Pdr.Api.Exceptions;

public class BillingBeforeKickoffException() 
: AppException("Billing start cannot be before the kickoff date.", StatusCodes.Status400BadRequest);
