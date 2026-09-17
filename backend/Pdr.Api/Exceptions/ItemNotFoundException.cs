using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class ItemNotFoundException : AppException
{
    public ItemNotFoundException(int id)
        : base($"Item with ID {id} not found.", StatusCodes.Status404NotFound)
    {
    }

    public ItemNotFoundException(string entityType, int id)
        : base($"{entityType} with ID {id} not found.", StatusCodes.Status404NotFound)
    {
    }
}
