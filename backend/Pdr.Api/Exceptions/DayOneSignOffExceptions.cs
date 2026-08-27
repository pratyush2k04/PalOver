using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class DayOneTechnicalBlockersRemainingException : AppException
{
    public DayOneTechnicalBlockersRemainingException()
        : base(EngagementValidationConstants.Messages.DayOneTechnicalBlockersRemaining, 400) { }
}

public class DayOneLeadershipExceptionForbiddenException : AppException
{
    public DayOneLeadershipExceptionForbiddenException()
        : base(EngagementValidationConstants.Messages.DayOneLeadershipExceptionForbidden, 403) { }
}

public class DayOneExceptionJustificationTooShortException : AppException
{
    public DayOneExceptionJustificationTooShortException()
        : base(EngagementValidationConstants.Messages.DayOneExceptionJustificationTooShort, 400) { }
}

public class EvidenceSnapshotNotFoundException : AppException
{
    public EvidenceSnapshotNotFoundException(int engagementId)
        : base(EngagementValidationConstants.Messages.EvidenceSnapshotNotFound(engagementId), 404) { }
}
