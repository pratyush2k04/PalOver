namespace Pdr.Api.Constants;

public static class EngagementValidationConstants
{
    public const string DescriptionRequired = "Engagement description is required.";
    public const string BillingStartDateRequired = "Billing start date is required.";

    public static class Warnings
    {
        public const string KickoffDateInPast = "Kickoff date is in the past.";
        public const string EngagementAlreadyExistsUpdated = "This engagement already exists - updated instead of duplicated.";
    }
}
