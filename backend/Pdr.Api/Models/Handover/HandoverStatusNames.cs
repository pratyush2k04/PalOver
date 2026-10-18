namespace Pdr.Api.Models.Handover;

public static class HandoverStatusNames
{
    public const string Draft = "Draft";
    public const string Submitted = "Submitted";
    public const string Reviewed = "Reviewed";

    public static bool IsSubmittedOrReviewed(string? statusName) =>
        statusName == Submitted || statusName == Reviewed;
}
