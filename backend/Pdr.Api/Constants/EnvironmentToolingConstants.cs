namespace Pdr.Api.Constants;

public static class EnvironmentToolingConstants
{
    public static class Roles
    {
        public const string TechLead = "Tech Lead";
        public const string ViewerRoles = "Tech Lead,Project Management,Leadership";
    }

    public static class Messages
    {
        public const string BlockReasonRequired = "A block reason must be provided when setting the status to Blocked.";

        public static string NotFound(int id) => $"Environment/tooling item with id {id} was not found.";
        public static string InvalidStatusTransition(string from, string to) => $"Cannot transition status from {from} to {to}.";
    }
}
