namespace Pdr.Api.DTOs.HandoverReadiness;

public class HandoverReadinessItemDto
{
    public int Id { get; set; }
    public string Area { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? BlockReason { get; set; }

    // Evidence tracking
    public int? EvidenceDocumentId { get; set; }
    public string? EvidenceDocumentFileName { get; set; }
    public DateTime? EvidenceSubmittedAt { get; set; }
    public int? EvidenceSubmittedById { get; set; }
    public string? EvidenceSubmittedByName { get; set; }
}

public record SaveHandoverReadinessEvidenceRequestDto(
    int DocumentId
);
