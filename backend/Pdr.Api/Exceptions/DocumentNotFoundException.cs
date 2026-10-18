using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class DocumentNotFoundException(int documentId)
    : AppException(DocumentValidationConstants.Messages.NotFound(documentId), StatusCodes.Status404NotFound);
