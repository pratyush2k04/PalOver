namespace Pdr.Api.Models.Handover;

public static class HandoverQueryFieldSectionNames
{
    public const string ClientBackground = "ClientBackground";
    public const string BusinessProblem = "BusinessProblem";
    public const string ClientSentiment = "ClientSentiment";
    public const string DealOrigin = "DealOrigin";
    public const string Commitment = "Commitment";

    // Sections whose queries point at another record via HandoverQuery.ReferenceId.
    // Add to this set (and HandoverQueryFieldSectionEnum) when a new referenceable
    // section is introduced (e.g. an access-tracker item) - no schema change needed.
    public static readonly IReadOnlySet<string> SectionsRequiringReference = new HashSet<string>
    {
        Commitment
    };
}
