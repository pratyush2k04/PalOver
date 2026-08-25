using Pdr.Api.Constants;

namespace Pdr.Api.Helpers;

public static class LeadershipDashboardHelpers
{
    public static int GetRequiredThreshold(int daysToKickoff)
    {
        if (daysToKickoff > 14)
            return LeadershipDashboardConstants.Thresholds.TierGreaterThan14;

        if (daysToKickoff >= 8)
            return LeadershipDashboardConstants.Thresholds.Tier8To14;

        if (daysToKickoff >= 4)
            return LeadershipDashboardConstants.Thresholds.Tier4To7;

        if (daysToKickoff >= 1)
            return LeadershipDashboardConstants.Thresholds.Tier1To3;

        return LeadershipDashboardConstants.Thresholds.TierDay0OrPast;
    }

    public static string DeriveStatus(int readinessScore, int openBlockerCount, int requiredThreshold)
    {
        if (readinessScore == 100 && openBlockerCount == 0)
            return LeadershipDashboardConstants.Statuses.KickoffReady;

        if (openBlockerCount > 0 || readinessScore < requiredThreshold)
            return LeadershipDashboardConstants.Statuses.AtRisk;

        return LeadershipDashboardConstants.Statuses.InProgress;
    }

    public static bool EvaluateResourcingGap(int? daysToBilling, int unconfirmedResourceCount) =>
        daysToBilling.HasValue &&
        daysToBilling.Value <= ResourceRequestConstants.BillingGapThresholdDays &&
        unconfirmedResourceCount > 0;
}
