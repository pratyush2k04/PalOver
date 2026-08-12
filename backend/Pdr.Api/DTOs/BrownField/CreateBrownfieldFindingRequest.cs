using Pdr.Api.Models.BrownField;

namespace Pdr.Api.DTOs.BrownField;

public class CreateBrownfieldFindingRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public BrownfieldSeverity Severity { get; set; } = BrownfieldSeverity.Low;
    public int? DocumentId { get; set; }
}
