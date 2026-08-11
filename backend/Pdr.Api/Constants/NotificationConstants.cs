namespace Pdr.Api.Constants;

public static class NotificationConstants
{
    public static class Messages
    {
        public static string NotFound(int id) => $"Notification with id {id} was not found.";
        public static string AccessDenied(int id) => $"You do not have access to notification with id {id}.";
        public static string TemplateNotFound(string key) => $"No notification template registered for key '{key}'.";
    }
}
