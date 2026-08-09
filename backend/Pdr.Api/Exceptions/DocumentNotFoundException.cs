using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class DocumentNotFoundException(int documentId)
    : AppException($"Document with id {documentId} was not found or already deleted.", StatusCodes.Status404NotFound);
