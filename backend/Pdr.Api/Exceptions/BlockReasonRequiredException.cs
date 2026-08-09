using Microsoft.AspNetCore.Http;
using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class BlockReasonRequiredException()
    : AppException(AccessTrackerConstants.Messages.BlockReasonRequired, StatusCodes.Status400BadRequest);
