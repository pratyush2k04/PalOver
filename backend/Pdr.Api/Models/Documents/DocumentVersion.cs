namespace Pdr.Api.Models.Documents;

public class DocumentVersion : BaseAuditableEntity
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int VersionNumber { get; set; }

    public required string FilePath { get; set; }

    public long FileSizeBytes { get; set; }

    public required string ContentType { get; set; }

    public string? ChangeNote { get; set; }

    public bool IsCurrent { get; set; }

    // Navigation property
    public Document Document { get; set; } = null!;
}
