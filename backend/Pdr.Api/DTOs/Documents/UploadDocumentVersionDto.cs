using Microsoft.AspNetCore.Http;

namespace Pdr.Api.DTOs.Documents;

public class UploadDocumentVersionDto
{
    public required IFormFile File { get; set; }

    public string? ChangeNote { get; set; }
}
