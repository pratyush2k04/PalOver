namespace Pdr.Api.Constants.Handovers;

public static class CreateCommitmentRequestConstants
{
    public const int MaxDescriptionLength = 2000;
    public const int MaxTitleLength = 100;
    public static readonly string[] ValidTimelineUnits = { "Days", "Weeks", "Months" };

    public const string DescriptionRequired = "Description is required.";
    public const string DescriptionMaxLength = "Description cannot exceed 2000 characters.";
    public const string CommitmentTypeIdRequired = "Commitment Type ID is required.";
    public const string TitleRequired = "Title is required.";
    public const string TitleMaxLength = "Title cannot exceed 100 characters.";
    public const string TimelineGreaterThanZero = "Timeline must be greater than 0.";
    public const string TimelineUnitInvalid = "Timeline Unit must be one of: Days, Weeks, Months.";
}
