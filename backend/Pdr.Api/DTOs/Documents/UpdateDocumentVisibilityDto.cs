namespace Pdr.Api.DTOs.Documents;

public class UpdateDocumentVisibilityDto
{
    public string? Visibility { get; set; }

    public List<int>? AllowedRoles { get; set; }

    public bool ResetToCategoryDefault { get; set; } = false;
}
