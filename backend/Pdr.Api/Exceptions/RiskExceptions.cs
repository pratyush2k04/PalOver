using Pdr.Api.Constants;

namespace Pdr.Api.Exceptions;

public class RiskNotFoundException : AppException
{
    public RiskNotFoundException(int riskId) 
        : base(RiskConstants.Messages.NotFound(riskId), 404) { }
}

public class ResolutionNoteRequiredException : AppException
{
    public ResolutionNoteRequiredException() 
        : base(RiskConstants.Messages.ResolutionNoteRequired, 400) { }
}

public class RiskCloseAccessDeniedException : AppException
{
    public RiskCloseAccessDeniedException() 
        : base(RiskConstants.Messages.RiskCloseAccessDenied, 403) { }
}

public class RiskCommentNotFoundException : AppException
{
    public RiskCommentNotFoundException(int commentId)
        : base(RiskConstants.Messages.CommentNotFound(commentId), 404) { }
}

public class RiskCommentEditWindowExpiredException : AppException
{
    public RiskCommentEditWindowExpiredException()
        : base(RiskConstants.Messages.CommentEditWindowExpired, 400) { }
}

public class RiskCommentEditForbiddenException : AppException
{
    public RiskCommentEditForbiddenException()
        : base(RiskConstants.Messages.CommentEditForbidden, 403) { }
}
