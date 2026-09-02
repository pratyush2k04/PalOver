namespace Pdr.Api.Constants;

public static class HandoverValidationConstants
{
    public const string AccessDenied = "You don't have access to Handover.";
    public const string AlreadySubmitted = "This handover has already been submitted and its commitments can no longer be changed.";
    public const string NotYetSubmitted = "This action requires the handover to be submitted first.";
    public const string NotFoundForSubmit = "Cannot submit a handover that does not exist.";
    public const string NotFoundForLeadershipAllocation = "Cannot allocate leadership for an engagement until its handover has been submitted.";
    public const string BusinessProblemRequiredSubmit = "Business Problem is required to submit.";
    public const string ClientSentimentRequiredSubmit = "Client Sentiment is required to submit.";
    public const string NotKickoffReadySubmit = "Engagement is not kickoff-ready. All mandatory items must be complete before submitting the handover.";
    public const string ReadinessChecklistNotGrantedSubmit = "All Readiness Checklist items must be Granted and none can be Blocked before submitting the handover.";
    public const string RealisticCommitmentRequiresSubmitted = "Realistic commitments can only be provided after the handover has been submitted.";
    public const string RealisticTimelineDiffersAcknowledgementRequired = "The realistic timeline differs from the promised timeline. Please provide an acknowledgement message.";
    public const string UpdatedAcknowledgementRequired = "The estimation gap between the promised and realistic timelines still exceeds the threshold. Please provide an updated acknowledgement message.";
    public const string MissingRealisticCommitmentsReview = "All commitments must have realistic estimates before marking handover as reviewed.";

    public static class Variance
    {
        public const double PercentageThreshold = 20.0;
        public const double MinimumWeekGapThreshold = 1.0;

        public static string FormatDescription(string committedText, string realisticText, int percentGap) =>
            $"Committed {committedText} vs realistic {realisticText} (+{percentGap}%)";
    }

    public static string ActionRequiresRole(string role) => $"This action requires the '{role}' role.";
    public static string ActionRequiresAnyRole(IEnumerable<string> roles) => $"This action requires one of the following roles: {string.Join(", ", roles)}.";
    public static string NotFoundForEngagement(int engagementId) => $"No handover draft exists yet for engagement {engagementId}. Save the Client Context section first.";
    public static string CommitmentNotFound(int id) => $"Commitment with ID {id} was not found.";
    public static string RealisticCommitmentNotFound(int id) => $"Realistic commitment for commitment ID {id} was not found.";
    public static string RealisticCommitmentAlreadyExists(int id) => $"Realistic commitment for commitment ID {id} already exists.";
}
