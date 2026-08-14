namespace Pdr.Api.DTOs.Risks;

public class RiskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Impact { get; set; } = string.Empty;
    public string Likelihood { get; set; } = string.Empty;

    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }

    public string? Mitigation { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ResolutionNote { get; set; }

    public int OpenForDays { get; set; }

    public string? SourceType { get; set; }
    public int? SourceItemId { get; set; }
    public string? SourceTitle { get; set; }
}
