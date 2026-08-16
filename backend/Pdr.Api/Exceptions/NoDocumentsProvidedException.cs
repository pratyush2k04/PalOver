using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class NoDocumentsProvidedException()
    : AppException(DocumentValidationConstants.Messages.NoDocumentsProvided, StatusCodes.Status400BadRequest);
