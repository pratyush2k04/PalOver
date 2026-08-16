namespace Pdr.Api.DTOs.Documents;

public class DocumentUploadResultDto
{
    public string? FileName { get; set; }

    public bool Success { get; set; }

    public DocumentResponseDto? Document { get; set; }

    public List<string> Errors { get; set; } = new();
}
