namespace Pdr.Api.Constants;

public static class EnvironmentToolingConstants
{
    public static class Routes
    {
        public const string Base = "api/engagements/{engagementId:int}/environment-tooling";
        public const string ItemsBase = "api/environment-tooling";
    }

    public static class Roles
    {
        public const string TechLead = "Tech Lead";
        public const string ViewerRoles = "Tech Lead,Project Management,Leadership";
    }

    public static class Messages
    {
        public const string NotApplicableReasonRequired = "NotApplicableReason is required when setting status to NotApplicable.";

        public static string NotFound(int id) => $"Environment/tooling item with id {id} was not found.";
        public static string InvalidStatusTransition(string from, string to) => $"Cannot transition status from {from} to {to}.";
    }
}
