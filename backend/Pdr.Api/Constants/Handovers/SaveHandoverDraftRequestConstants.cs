namespace Pdr.Api.Constants.Handovers;

public static class SaveHandoverDraftRequestConstants
{
    public const int MaxFieldLength = 2000;
    public static readonly string[] ValidSentiments = { "New", "Warm", "Strained" };

    public const string ClientBackgroundMaxLength = "Client Background cannot exceed 2000 characters.";
    public const string BusinessProblemMaxLength = "Business Problem cannot exceed 2000 characters.";
    public const string ClientSentimentInvalid = "Client Sentiment must be one of: New, Warm, Strained.";
    public const string DealOriginMaxLength = "Deal Origin cannot exceed 2000 characters.";
}
