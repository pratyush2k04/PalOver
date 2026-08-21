using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class EngagementNotKickoffReadyException : AppException
{
    public EngagementNotKickoffReadyException()
        : base(EngagementValidationConstants.Messages.EngagementNotKickoffReady, 400) { }
}

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
