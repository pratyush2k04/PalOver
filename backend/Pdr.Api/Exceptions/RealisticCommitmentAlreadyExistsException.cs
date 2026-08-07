using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class RealisticCommitmentAlreadyExistsException(int commitmentId)
    : AppException($"A realistic commitment already exists for commitment {commitmentId}.", StatusCodes.Status409Conflict);
