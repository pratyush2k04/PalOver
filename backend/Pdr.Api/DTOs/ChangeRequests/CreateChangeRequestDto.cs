namespace Pdr.Api.DTOs.ChangeRequests;

public class CreateChangeRequestDto
{
    public string Description { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
}
