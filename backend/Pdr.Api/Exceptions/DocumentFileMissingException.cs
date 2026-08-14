using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class DocumentFileMissingException()
    : AppException(DocumentValidationConstants.Messages.FileNotFound, StatusCodes.Status404NotFound);
