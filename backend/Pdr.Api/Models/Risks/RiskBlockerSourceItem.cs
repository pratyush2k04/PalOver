namespace Pdr.Api.Models.Risks;

public record RiskBlockerSourceItem(
    string SourceType,
    int SourceItemId,
    string Title,
    string Reason,
    int? OwnerId
);
