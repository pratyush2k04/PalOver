namespace Pdr.Api.Exceptions;

public class CommunicationCadenceNotFoundException(int id)
    : AppException($"Communication cadence with ID '{id}' was not found.", 404);
