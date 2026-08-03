namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterChecklistVersionSummaryResponseDto
{
    public long Id { get; set; }

    public int VersionNumber { get; set; }

    public long StatusId { get; set; }

    public required string StatusName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
