namespace Pdr.Api.Constants;

public static class AuditConstants
{
    public static class Messages
    {
        public static string ParsePayloadFailed(string action) => $"{action} audit payload could not be parsed.";
        public static string ActionNotFound(string action) => $"No AuditAction row found for {action}.";
        public static string MapperNotFound(string actionKey) => $"No IAuditTemplateMapper registered for action {actionKey}.";
    }
}
