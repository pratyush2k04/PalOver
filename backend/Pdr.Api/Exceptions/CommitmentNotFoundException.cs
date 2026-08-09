using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Exceptions;

public class CommitmentNotFoundException(int commitmentId)
    : AppException(HandoverValidationConstants.CommitmentNotFound(commitmentId), StatusCodes.Status404NotFound);
