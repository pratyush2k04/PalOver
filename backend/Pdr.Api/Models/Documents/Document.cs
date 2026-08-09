using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.Documents;

public class Document : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public int CategoryId { get; set; }

    public required string FileName { get; set; }

    public required string FilePath { get; set; }

    public long FileSizeBytes { get; set; }

    public required string ContentType { get; set; }

    public string? DeletionReason { get; set; }

    public string Visibility { get; set; } = "Team-wide";

    public List<int> AllowedRoles { get; set; } = new();

    // Navigation properties
    public DocumentCategory Category { get; set; } = null!;
    
    public Engagement Engagement { get; set; } = null!;
}
