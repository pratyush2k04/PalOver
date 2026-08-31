namespace Pdr.Api.Constants;

public static class CreateRealisticCommitmentRequestConstants
{
    public const int MaxDescriptionLength = 2000;

    public const string DescriptionRequired = "Description is required.";
    public const string DescriptionMaxLength = "Description cannot exceed 2000 characters.";
    public const string TimelineGreaterThanZero = "Timeline must be greater than 0.";
    public const string TimelineUnitInvalid = "Timeline Unit must be one of: Days, Weeks, Months.";
    public const string InvalidRequest = "Invalid realistic commitment request.";
}
