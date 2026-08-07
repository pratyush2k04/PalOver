using Microsoft.AspNetCore.Http;

namespace Pdr.Api.DTOs.Documents;

public class DocumentUploadDto
{
    public required IFormFile File { get; set; }

    public int EngagementId { get; set; }

    public int CategoryId { get; set; }

    public string Visibility { get; set; } = "Team-wide";
}
