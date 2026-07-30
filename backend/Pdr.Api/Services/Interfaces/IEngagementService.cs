using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementService
{
    Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request);
}
