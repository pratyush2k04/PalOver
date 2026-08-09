namespace Pdr.Api.Constants.Handovers;

public static class HandoverValidationConstants
{
    public const string AccessDenied = "You don't have access to Handover.";
    public const string AlreadySubmitted = "This handover has already been submitted and its commitments can no longer be changed.";
    public const string NotFoundForSubmit = "Cannot submit a handover that does not exist.";
    public const string BusinessProblemRequiredSubmit = "Business Problem is required to submit.";
    public const string ClientSentimentRequiredSubmit = "Client Sentiment is required to submit.";
    public const string UpdatedAcknowledgementRequired = "The estimation gap between the promised and realistic timelines still exceeds the threshold. Please provide an updated acknowledgement message.";

    public static string ActionRequiresRole(string role) => $"This action requires the '{role}' role.";
}
