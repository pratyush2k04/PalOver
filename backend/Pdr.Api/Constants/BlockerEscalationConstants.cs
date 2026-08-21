namespace Pdr.Api.Constants;

public static class BlockerEscalationConstants
{
    public static class BackgroundService
    {
        public const int CheckIntervalHours = 1;
        public const string ServiceStarted = "Blocker Escalation Background Service started.";
        public const string ProcessedEscalations = "Blocker Escalation Service processed {Count} escalations.";
    }

    public static class Messages
    {
        public static string AcceleratedDailyEscalation(int daysToKickoff) =>
            $"ACCELERATED DAILY ESCALATION: Kickoff in {Math.Max(0, daysToKickoff)} days. Blocker remains unresolved.";

        public static string UnresolvedPm(int openWorkingDays) =>
            $"Blocker unresolved for {openWorkingDays} working days (> 2 working days).";

        public static string UnresolvedLeadership(int daysSinceLastEscalation) =>
            $"Blocker unresolved for a further {daysSinceLastEscalation} working days after PM escalation.";
    }

    public static class Routes
    {
        public static string RisksWorkspaceLink(int engagementId) =>
            $"/engagements/{engagementId}/workspace/risks";
    }
}
