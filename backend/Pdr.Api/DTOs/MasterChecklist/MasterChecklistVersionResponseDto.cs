namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterChecklistVersionResponseDto
{
    public int Id { get; set; }

    public int VersionNumber { get; set; }

    public int StatusId { get; set; }

    public required string StatusName { get; set; }

    public DateTime UpdatedAt { get; set; }

    public required List<MasterChecklistItemResponseDto> Items { get; set; }
}
