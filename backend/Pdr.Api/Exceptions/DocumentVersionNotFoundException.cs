using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class DocumentVersionNotFoundException()
    : AppException(DocumentValidationConstants.Messages.VersionNotFound, StatusCodes.Status404NotFound);
