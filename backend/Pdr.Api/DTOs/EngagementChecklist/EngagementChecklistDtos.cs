namespace Pdr.Api.DTOs.EngagementChecklist;

public record EngagementChecklistResponseDto(
    int Id,
    int EngagementId,
    int SourceMasterVersionId,
    int VersionNumber,
    int PendingExemptionsCount,
    List<MandatoryIncompleteItemDto> IncompleteMandatoryItems,
    List<EngagementChecklistCategoryGroupDto> Categories
);

public record MandatoryIncompleteItemDto(
    int Id,
    string Title,
    string Status,
    string CategoryName
);

public record EngagementChecklistCategoryGroupDto(
    int CategoryId,
    string CategoryName,
    List<EngagementChecklistItemDto> Items
);

public record EngagementChecklistItemDto(
    int Id,
    int EngagementChecklistId,
    int? SourceMasterItemId,
    string Title,
    string? Description,
    int CategoryId,
    string CategoryName,
    bool IsMandatory,
    int DisplayOrder,
    bool IsApplicable,
    string Status,
    string? NotApplicableReason,
    string ExemptionStatus,
    int? ExemptionRequestedById,
    DateTime? ExemptionRequestedAt,
    int? ExemptionReviewedById,
    DateTime? ExemptionReviewedAt,
    string? ExemptionRejectionReason
);

public record UpdateItemStatusRequestDto(
    string Status
);

public record UpdateItemApplicabilityRequestDto(
    bool IsApplicable,
    string? Reason
);

public record ReviewExemptionRequestDto(
    bool Approved,
    string? RejectionReason
);
