namespace Pdr.Api.Exceptions;

public class HandoverReadinessItemNotFoundException : AppException
{
    public HandoverReadinessItemNotFoundException(int id)
        : base($"Handover readiness item with ID {id} was not found.", 404)
    {
    }
}
