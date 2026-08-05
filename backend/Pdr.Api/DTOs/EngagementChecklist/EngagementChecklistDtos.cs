namespace Pdr.Api.DTOs.EngagementChecklist;

public record EngagementChecklistResponseDto(
    int Id,
    int EngagementId,
    int SourceMasterVersionId,
    int VersionNumber,
    List<EngagementChecklistCategoryGroupDto> Categories
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
    string Status
);
