namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterChecklistVersionResponseDto
{
    public long Id { get; set; }

    public int VersionNumber { get; set; }

    public long StatusId { get; set; }

    public required string StatusName { get; set; }

    public required List<MasterChecklistItemResponseDto> Items { get; set; }
}
