using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class AccessItemNotFoundException(int accessItemId)
    : AppException($"Access item with id {accessItemId} was not found.", StatusCodes.Status404NotFound);
