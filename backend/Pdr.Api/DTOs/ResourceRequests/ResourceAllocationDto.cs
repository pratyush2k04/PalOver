namespace Pdr.Api.DTOs.ResourceRequests;

public class ResourceAllocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public DateOnly? ConfirmedDate { get; set; }
}
