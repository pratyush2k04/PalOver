namespace Pdr.Api.Constants;

public static class AccessTrackerConstants
{
    public static class Roles
    {
        public const string TechLead = "Tech Lead";
        public const string ViewerRoles = "Tech Lead,Project Management,Leadership";
    }

    public static class Messages
    {
        public const string BlockReasonRequired = "A block reason must be provided when setting the status to Blocked.";
        public const string RiskEvaluationCompleted = "Client-side risk evaluation completed.";

        public static string NotFound(int id) => $"Access item with id {id} was not found.";
        public static string InvalidStatusTransition(string from, string to) => $"Cannot transition access item from '{from}' to '{to}'.";
    }
}
