namespace Pdr.Api.DTOs.TechnicalSummary;

public class DayOneBlockerItemDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public string TargetSection { get; set; } = string.Empty;
}
