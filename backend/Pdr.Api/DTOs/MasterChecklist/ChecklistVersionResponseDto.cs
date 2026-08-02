namespace Pdr.Api.DTOs.MasterChecklist;

public class ChecklistVersionResponseDto
{
    public long Id { get; set; }

    public int VersionNumber { get; set; }

    public long StatusId { get; set; }

    public required string StatusName { get; set; }

    public required List<ChecklistItemResponseDto> Items { get; set; }
}
