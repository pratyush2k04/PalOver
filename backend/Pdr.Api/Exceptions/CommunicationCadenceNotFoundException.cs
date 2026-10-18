using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class CommunicationCadenceNotFoundException(int id)
    : AppException(CadenceConstants.Messages.NotFound(id), StatusCodes.Status404NotFound);
