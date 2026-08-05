namespace Pdr.Api.DTOs.MasterChecklist;

public class UpdateChecklistItemRequestDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public bool IsMandatory { get; set; } = true;
}
