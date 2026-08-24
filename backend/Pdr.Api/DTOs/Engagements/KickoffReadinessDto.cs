namespace Pdr.Api.DTOs.Engagements;

public record KickoffReadinessDto(
    int Score,
    int TotalApplicable,
    int TotalPassed,
    int HandoverFieldsApplicable,
    int HandoverFieldsPassed,
    int CommitmentsApplicable,
    int CommitmentsPassed,
    int AccessItemsApplicable,
    int AccessItemsPassed,
    int ChecklistItemsApplicable,
    int ChecklistItemsPassed);
