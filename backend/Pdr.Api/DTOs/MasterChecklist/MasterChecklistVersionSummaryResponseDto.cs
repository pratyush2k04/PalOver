namespace Pdr.Api.DTOs.MasterChecklist;

public class MasterChecklistVersionSummaryResponseDto
{
    public int Id { get; set; }

    public int VersionNumber { get; set; }

    public int StatusId { get; set; }

    public required string StatusName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
