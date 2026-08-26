namespace Pdr.Api.Constants;

public static class LeadershipDashboardConstants
{
    public static class Statuses
    {
        public const string KickoffReady = "Kickoff-ready";
        public const string InProgress = "In progress";
        public const string AtRisk = "At risk";
    }

    public static class Thresholds
    {
        public const int TierGreaterThan14 = 0;
        public const int Tier8To14 = 40;
        public const int Tier4To7 = 60;
        public const int Tier1To3 = 85;
        public const int TierDay0OrPast = 95;
    }

    public static class Days
    {
        public const int TierGreaterThan14 = 14;
        public const int Tier8To14 = 8;
        public const int Tier4To7 = 4;
        public const int Tier1To3 = 1;
    }
}
