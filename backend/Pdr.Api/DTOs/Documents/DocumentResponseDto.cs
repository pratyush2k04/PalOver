namespace Pdr.Api.DTOs.Documents;

public class DocumentResponseDto
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public string Visibility { get; set; } = "Team-wide";

    public List<int> AllowedRoles { get; set; } = new();

    public bool IsCustomVisibility => Visibility == "Custom";

    public string CategoryDefaultVisibility { get; set; } = "Team-wide";

    public int CurrentVersionNumber { get; set; }
}
