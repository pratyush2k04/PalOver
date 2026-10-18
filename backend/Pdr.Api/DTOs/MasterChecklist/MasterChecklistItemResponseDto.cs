namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterChecklistItemResponseDto
{
    public int Id { get; set; }

    public int VersionId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public required string CategoryName { get; set; }

    public bool IsMandatory { get; set; }
    public bool EvidenceRequired { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRetired { get; set; }
}
