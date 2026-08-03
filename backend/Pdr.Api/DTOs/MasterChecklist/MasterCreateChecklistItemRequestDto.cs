namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterCreateChecklistItemRequestDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public long CategoryId { get; set; }

    public bool IsMandatory { get; set; } = true;

    public int DisplayOrder { get; set; }
}
