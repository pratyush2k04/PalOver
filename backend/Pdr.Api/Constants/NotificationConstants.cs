namespace Pdr.Api.Constants;

public static class NotificationConstants
{
    public static class TemplateKeys
    {
        public const string BlockerEscalatedPM = "BlockerEscalatedPM";
        public const string BlockerEscalatedLeadership = "BlockerEscalatedLeadership";
        public const string AccessItemBlocked = "AccessItemBlocked";
        public const string PMAllocated = "PMAllocated";
        public const string HandoverSubmitted = "HandoverSubmitted";
        public const string HandoverDraftFirstSaved = "HandoverDraftFirstSaved";
        public const string ResourceRequestRaised = "ResourceRequestRaised";
        public const string RiskCommentMention = "RiskCommentMention";
        public const string LeadershipAssigned = "LeadershipAssigned";
        public const string LeadershipActionRequired = "LeadershipActionRequired";
        public const string PMAssigned = "PMAssigned";
        public const string TechLeadAssigned = "TechLeadAssigned";
        public const string DocumentUploaded = "DocumentUploaded";
        public const string ResourcingGapFlagRaised = "ResourcingGapFlagRaised";
        public const string CommitmentVarianceEscalated = "CommitmentVarianceEscalated";
        public const string PresalesCommitmentAdded = "PresalesCommitmentAdded";
        public const string PendingCommitmentsSummary = "PendingCommitmentsSummary";
    }

    public static class RiskCommentMention
    {
        public const string TemplateKey = "RiskCommentMention";
        public const string ParseErrorMessage = "RiskCommentMention notification data could not be parsed.";
        public const string RiskTitleKey = "RiskTitle";
        public const string AuthorNameKey = "AuthorName";
        public const string CommentSnippetKey = "CommentSnippet";
        public const string DefaultRiskTitle = "Risk";
        public const string DefaultAuthorName = "A team member";
    }

    public static class CommitmentVarianceEscalated
    {
        public const string TemplateKey = "CommitmentVarianceEscalated";
        public const string ParseErrorMessage = "CommitmentVarianceEscalated notification data could not be parsed.";
        public const string EngagementNameKey = "EngagementName";
        public const string CommitmentTitleKey = "CommitmentTitle";
        public const string VarianceDescriptionKey = "VarianceDescription";
        public const string PercentGapKey = "PercentGap";
        public const string WeekGapKey = "WeekGap";
        public const string EngagementIdKey = "EngagementId";
        public const string DefaultEngagementName = "Engagement";
    }

    public static class Messages
    {
        public static string NotFound(int id) => $"Notification with id {id} was not found.";
        public static string AccessDenied(int id) => $"You do not have access to notification with id {id}.";
        public static string TemplateNotFound(string key) => $"No notification template registered for key '{key}'.";
        public static string NotYetScheduled(int id) => $"Notification with id {id} cannot be marked as read before its scheduled time.";
        public static string InvalidPayloadData(string key) => $"Notification data for '{key}' could not be parsed.";
        public static string ParsePayloadFailed(string templateKey) => $"{templateKey} notification data could not be parsed.";
    }
}
