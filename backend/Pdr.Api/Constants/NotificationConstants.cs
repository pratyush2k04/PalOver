namespace Pdr.Api.Constants;

public static class NotificationConstants
{
    public static class TemplateKeys
    {
        public const string AccessItemBlocked = "AccessItemBlocked";
        public const string PMAllocated = "PMAllocated";
        public const string HandoverSubmitted = "HandoverSubmitted";
        public const string RiskCommentMention = "RiskCommentMention";
        public const string LeadershipAssigned = "LeadershipAssigned";
        public const string PMAssigned = "PMAssigned";
        public const string TechLeadAssigned = "TechLeadAssigned";
        public const string DocumentUploaded = "DocumentUploaded";
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

    public static class Messages
    {
        public static string NotFound(int id) => $"Notification with id {id} was not found.";
        public static string AccessDenied(int id) => $"You do not have access to notification with id {id}.";
        public static string TemplateNotFound(string key) => $"No notification template registered for key '{key}'.";
        public static string NotYetScheduled(int id) => $"Notification with id {id} cannot be marked as read before its scheduled time.";
        public static string ParsePayloadFailed(string templateKey) => $"{templateKey} notification data could not be parsed.";
    }
}
