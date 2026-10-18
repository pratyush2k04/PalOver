using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.DTOs.Engagements;

public record MarkSowSignedResponse(Engagement Engagement, List<string> Warnings);
