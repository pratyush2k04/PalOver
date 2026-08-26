namespace Pdr.Api.DTOs.Engagements;

public record HandoverDraftStatusDto(
    bool HasHandoverDraft,
    string? Status,
    string? ClientBackground,
    string? BusinessProblem,
    string? ClientSentiment);
