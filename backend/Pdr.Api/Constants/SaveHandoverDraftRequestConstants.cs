namespace Pdr.Api.Constants;

public static class SaveHandoverDraftRequestConstants
{
    public const int MaxFieldLength = 500;
    public static readonly string[] ValidSentiments = { "New", "Warm", "Strained" };

    public const string ClientBackgroundMaxLength = "Client Background cannot exceed 500 characters.";
    public const string BusinessProblemMaxLength = "Business Problem cannot exceed 500 characters.";
    public const string ClientSentimentInvalid = "Client Sentiment must be one of: New, Warm, Strained.";
    public const string DealOriginMaxLength = "Deal Origin cannot exceed 500 characters.";
}
