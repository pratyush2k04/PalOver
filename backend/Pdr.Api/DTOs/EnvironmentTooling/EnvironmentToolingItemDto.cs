using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.DTOs.EnvironmentTooling
{
    public class EnvironmentToolingItemDto
    {
        public int EnvironmentToolingItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? EnvironmentToolingDefinitionId { get; set; }
        public int EngagementId { get; set; }
        public int OwnerId { get; set; }
        public string OwnerFullName { get; set; } = string.Empty;
        public string? CustomName { get; set; }
        public EnvironmentToolingStatus Status { get; set; }
        public string? NotApplicableReason { get; set; }
        public string? BlockReason { get; set; }
    }
}
