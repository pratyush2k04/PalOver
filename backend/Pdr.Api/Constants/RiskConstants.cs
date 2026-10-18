namespace Pdr.Api.Constants;

public static class RiskConstants
{
    public static class Validation
    {
        public const string TitleRequired = "Risk title is required.";
        public const string TitleMaxLength = "Risk title must not exceed 300 characters.";

        public const string DescriptionRequired = "Risk description is required.";
        public const string DescriptionMaxLength = "Risk description must not exceed 500 characters.";

        public const string ImpactInvalid = "Impact must be a valid RiskImpact value (Low, Medium, High).";
        public const string LikelihoodInvalid = "Likelihood must be a valid RiskLikelihood value (Low, Medium, High).";
        public const string StatusInvalid = "Status must be a valid RiskStatus value.";

        public const string OwnerRequired = "Risk owner is required.";

        public const string MitigationMaxLength = "Mitigation details must not exceed 500 characters.";

        public const string ResolutionNoteRequired = "Resolution note is required when closing a risk.";
        public const string ResolutionNoteMaxLength = "Resolution note must not exceed 500 characters.";

        public const string CommentContentRequired = "Comment content is required.";
        public const string CommentContentMaxLength = "Comment content must not exceed 1000 characters.";
    }

    public static class Messages
    {
        public const string ResolutionNoteRequired = "A resolution note is required when closing a risk.";
        public const string RiskCloseAccessDenied = "Only a Project Manager or the assigned Risk Owner can close this risk.";
        public const string CommentEditWindowExpired = "Comments can only be edited within 15 minutes of creation.";
        public const string CommentEditForbidden = "You can only edit your own comments.";

        public static string NotFound(int id) => $"Risk with ID {id} was not found.";
        public static string CommentNotFound(int id) => $"Risk comment with ID {id} was not found.";
    }

    public static class SourceTypes
    {
        public const string Checklist = "Checklist";
        public const string AccessTracker = "AccessTracker";
        public const string EnvironmentTooling = "EnvironmentTooling";
        public const string ResourceRequest = "ResourceRequest";
        public const string HandoverReadiness = "HandoverReadiness";
    }
}
