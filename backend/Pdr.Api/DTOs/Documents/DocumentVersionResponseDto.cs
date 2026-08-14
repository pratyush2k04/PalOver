namespace Pdr.Api.DTOs.Documents;

public class DocumentVersionResponseDto
{
    public int Id { get; set; }

    public int VersionNumber { get; set; }

    public bool IsCurrent { get; set; }

    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public string? ChangeNote { get; set; }
}
