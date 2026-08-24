using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.DTOs.AccessTracker;

public record AccessItemDelayInfoDto(int Id, int EngagementId, AccessItemStatus Status, DateTime CreatedAt, DateOnly? GrantedDate);
