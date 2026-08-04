using Microsoft.AspNetCore.Http;

namespace Pdr.Api.Exceptions;

public class CommitmentNotFoundException(long commitmentId)
    : AppException($"Commitment with id {commitmentId} was not found.", StatusCodes.Status404NotFound);
