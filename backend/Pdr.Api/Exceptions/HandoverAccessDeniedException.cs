using System;

namespace Pdr.Api.Exceptions;

public class HandoverAccessDeniedException : Exception
{
    public HandoverAccessDeniedException(string message) : base(message)
    {
    }
}
