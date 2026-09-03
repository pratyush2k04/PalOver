namespace Pdr.Api.Constants;

public static class ChangeRequestConstants
{
    public const int MaxDescriptionLength = 500;
    public const int MaxImpactLength = 500;
    public const int MaxRejectionReasonLength = 500;

    public static class ValidationMessages
    {
        public const string DescriptionRequired = "Description is required.";
        public const string DescriptionMaxLength = "Description cannot exceed 500 characters.";
        public const string ImpactRequired = "Impact estimate is required.";
        public const string ImpactMaxLength = "Impact estimate cannot exceed 500 characters.";
        public const string RejectionReasonRequired = "Reason is required when rejecting a change request.";
        public const string RejectionReasonMaxLength = "Rejection reason cannot exceed 500 characters.";
        public const string SelfApprovalNotAllowed = "The user who raised the change request cannot approve or reject it.";

        public static string NotFound(int id) => $"Change request with ID {id} was not found.";
    }

    public static class StatusDisplayNames
    {
        public const string UnderReview = "Under Review";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}
