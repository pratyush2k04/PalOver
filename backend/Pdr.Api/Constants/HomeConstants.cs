namespace Pdr.Api.Constants;

public static class HomeConstants
{

    public static class CardTypes
    {
        public const string Engagement = "engagement";
        public const string DraftHandover = "draft-handover";
        public const string CompleteHandover = "complete-handover";
        public const string Checklist = "checklist";
        public const string Blocker = "blocker";
        public const string TechQueue = "tech-queue";
        public const string AccessItem = "access-item";
        public const string Escalation = "escalation";
    }

    public static class Fallbacks
    {
        public const string Task = "Task";
        public const string System = "System";
    }

    public static class Statuses
    {
        public const string Incomplete = "Incomplete";
    }
}
