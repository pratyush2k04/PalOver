namespace Pdr.Api.Constants;

public static class HomeConstants
{
    public static class FrontendRoutes
    {
        public const string EngagementDashboard = "/engagement-dashboard";
        public const string MyHandovers = "/my-handovers";
        public const string TechnicalReadiness = "/technical-readiness";
        public const string PortfolioDashboard = "/portfolio-dashboard";
    }

    public static class Titles
    {
        public const string MyEngagements = "My Engagements";
        public const string MyHandovers = "My Handovers";
        public const string TechnicalReadiness = "Technical Readiness";
        public const string PortfolioDashboard = "Portfolio Dashboard";
    }

    public static class EmptyStates
    {
        public const string MyEngagements = "You have no engagements yet.";
        public const string MyHandovers = "You have no handovers yet.";
        public const string TechnicalReadiness = "You have no technical readiness tasks yet.";
        public const string PortfolioDashboard = "No portfolio items available.";
    }

    public static class CardTypes
    {
        public const string Engagement = "engagement";
        public const string DraftHandover = "draft-handover";
        public const string CompleteHandover = "complete-handover";
        public const string Checklist = "checklist";
        public const string Blocker = "blocker";
        public const string TechQueue = "tech-queue";
        public const string AccessItem = "access-item";
        public const string Escalation = "escalation";
    }

    public static class Fallbacks
    {
        public const string Task = "Task";
        public const string System = "System";
    }

    public static class Statuses
    {
        public const string Incomplete = "Incomplete";
    }
}
