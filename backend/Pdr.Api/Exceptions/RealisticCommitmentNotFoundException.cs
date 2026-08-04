using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class RealisticCommitmentNotFoundException(long commitmentId)
    : AppException($"No realistic commitment exists yet for commitment {commitmentId}.", StatusCodes.Status404NotFound);
