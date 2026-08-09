using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Exceptions;

public class RealisticCommitmentAlreadyExistsException(int commitmentId)
    : AppException(HandoverValidationConstants.RealisticCommitmentAlreadyExists(commitmentId), StatusCodes.Status400BadRequest);
