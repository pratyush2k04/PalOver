namespace Pdr.Api.Constants;

public static class HomeConstants
{
    public static class FrontendRoutes
    {
        public const string UserDirectory = "/user-directory";
        public const string EngagementDashboard = "/engagement-dashboard";
        public const string MyHandovers = "/my-handovers";
        public const string TechnicalReadiness = "/technical-readiness";
        public const string PortfolioDashboard = "/portfolio-dashboard";
    }

    public static class Titles
    {
        public const string UserDirectory = "User Directory";
        public const string MyEngagements = "My Engagements";
        public const string MyHandovers = "My Handovers";
        public const string TechnicalReadiness = "Technical Readiness";
        public const string PortfolioDashboard = "Portfolio Dashboard";
    }

    public static class EmptyStates
    {
        public const string UserDirectory = "No users found.";
        public const string MyEngagements = "You have no engagements yet.";
        public const string MyHandovers = "You have no handovers yet.";
        public const string TechnicalReadiness = "You have no technical readiness tasks yet.";
        public const string PortfolioDashboard = "No portfolio items available.";
    }
}
