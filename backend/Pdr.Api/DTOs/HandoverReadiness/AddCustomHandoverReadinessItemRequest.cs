namespace Pdr.Api.DTOs.HandoverReadiness;

public class AddCustomHandoverReadinessItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Area { get; set; }
}
