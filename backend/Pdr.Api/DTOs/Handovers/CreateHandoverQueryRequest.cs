namespace Pdr.Api.DTOs.Handovers;

public class CreateHandoverQueryRequest
{
    public string FieldSection { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string Question { get; set; } = string.Empty;
}
