using Microsoft.AspNetCore.Http;
namespace Pdr.Api.Exceptions;

public class ClientNameRequiredException()
: AppException("Client name is required.", StatusCodes.Status400BadRequest);
