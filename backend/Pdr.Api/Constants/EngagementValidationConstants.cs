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

    public static class Messages
    {
        public const string BillingBeforeKickoff = "Billing start cannot be before the kickoff date.";
        public const string ClientNameRequired = "Client name is required.";
        public const string EngagementNameRequired = "Engagement name is required.";
        
        public static string NotFound(int id) => $"Engagement with id {id} was not found.";
        public static string InvalidType(int id) => $"Engagement type with id {id} does not exist.";
        public static string InvalidCommercialModel(int id) => $"Commercial model with id {id} does not exist.";
        public static string InvalidOwnerRole(int userId, string roleName) => $"User {userId} does not have the required role '{roleName}'.";
    }
}
