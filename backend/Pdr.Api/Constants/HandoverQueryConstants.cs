namespace Pdr.Api.Constants;

public static class HandoverQueryConstants
{
    public const int MaxQuestionLength = 2000;
    public const int MaxResponseLength = 2000;

    public const string QuestionRequired = "Question is required.";
    public const string QuestionMaxLength = "Question cannot exceed 2000 characters.";
    public const string FieldSectionInvalid = "Field section is not a recognized handover section.";
    public const string ReferenceIdRequired = "A reference is required for this field section.";
    public const string ReferenceIdNotAllowed = "A reference is not applicable for this field section.";
    public const string ResponseTextRequired = "Response is required.";
    public const string ResponseTextMaxLength = "Response cannot exceed 2000 characters.";
    public const string AlreadyResponded = "This query has already been responded to and cannot be changed.";
    public const string InvalidRequest = "Invalid handover query request.";
    public const string InvalidResponse = "Invalid response.";

    public static string NotFound(int queryId) => $"Handover query with ID {queryId} was not found.";
}
