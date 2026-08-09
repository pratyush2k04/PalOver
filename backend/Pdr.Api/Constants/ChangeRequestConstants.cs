namespace Pdr.Api.Constants;

public static class ChangeRequestConstants
{
    public const int MaxDescriptionLength = 2000;
    public const int MaxImpactLength = 2000;
    public const int MaxRejectionReasonLength = 2000;

    public static class ValidationMessages
    {
        public const string DescriptionRequired = "Description is required.";
        public const string DescriptionMaxLength = "Description cannot exceed 2000 characters.";
        public const string ImpactRequired = "Impact estimate is required.";
        public const string ImpactMaxLength = "Impact estimate cannot exceed 2000 characters.";
        public const string RejectionReasonRequired = "Reason is required when rejecting a change request.";
        public const string RejectionReasonMaxLength = "Rejection reason cannot exceed 2000 characters.";
    }
}
