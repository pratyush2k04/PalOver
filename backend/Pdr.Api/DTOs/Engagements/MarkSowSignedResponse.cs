using Pdr.Api.Models;

namespace Pdr.Api.DTOs.Engagements;

public record MarkSowSignedResponse(Engagement Engagement, List<string> Warnings);
