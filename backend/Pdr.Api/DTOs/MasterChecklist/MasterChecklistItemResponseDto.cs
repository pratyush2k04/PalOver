namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterChecklistItemResponseDto
{
    public long Id { get; set; }

    public long VersionId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public long CategoryId { get; set; }

    public required string CategoryName { get; set; }

    public bool IsMandatory { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRetired { get; set; }
}
