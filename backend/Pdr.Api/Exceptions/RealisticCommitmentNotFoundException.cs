using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Exceptions;

public class RealisticCommitmentNotFoundException(int commitmentId)
    : AppException(HandoverValidationConstants.RealisticCommitmentNotFound(commitmentId), StatusCodes.Status404NotFound);
