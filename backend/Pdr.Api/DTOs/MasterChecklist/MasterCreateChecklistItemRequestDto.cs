namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterCreateChecklistItemRequestDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public bool IsMandatory { get; set; } = true;
    public bool EvidenceRequired { get; set; } = false;

    public int DisplayOrder { get; set; }
}
