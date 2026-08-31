namespace Pdr.Api.Constants;

public static class HandoverQueryConstants
{
    public const int MaxQuestionLength = 2000;
    public const int MaxResponseLength = 2000;
    public const int MaxMessageLength = 2000;

    public const string QuestionRequired = "Question is required.";
    public const string QuestionMaxLength = "Question cannot exceed 2000 characters.";
    public const string FieldSectionInvalid = "Field section is not a recognized handover section.";
    public const string ReferenceIdRequired = "A reference is required for this field section.";
    public const string ReferenceIdNotAllowed = "A reference is not applicable for this field section.";
    public const string MessageTextRequired = "Message text is required.";
    public const string MessageTextMaxLength = "Message text cannot exceed 2000 characters.";
    public const string QueryAlreadyResolved = "Cannot add messages to a resolved query.";
    public const string InvalidTurnExpectedResponse = "A response from Pre Sales is expected before adding another question.";
    public const string InvalidTurnExpectedQuestion = "A follow-up question from Project Management is expected before adding another response.";
    public const string CannotResolvePendingQuestion = "Cannot resolve a query while a question is pending a response from Pre Sales.";
    public const string CannotResolveWithoutResponse = "Cannot resolve a query that has not received any responses.";

    public static string NotFound(int queryId) => $"Handover query with ID {queryId} was not found.";
}
