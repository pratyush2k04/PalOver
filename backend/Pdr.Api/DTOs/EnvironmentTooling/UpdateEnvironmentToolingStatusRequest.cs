using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.DTOs.EnvironmentTooling
{
    public class UpdateEnvironmentToolingStatusRequest
    {
        public EnvironmentToolingStatus Status { get; set; }
        public string? NotApplicableReason { get; set; }
        public string? BlockReason { get; set; }
    }
}
