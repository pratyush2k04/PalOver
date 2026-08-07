namespace Pdr.Api.Constants;

public static class MasterChecklistConstants
{
    public static class Routes
    {
        public const string Base = "api/[controller]";
        public const string CurrentDraft = "current-draft";
        public const string Published = "published";
        public const string Versions = "versions";
        public const string Categories = "categories";
        public const string Items = "items";
    }

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
    }

    public static class Roles
    {
        public const string ViewerRoles = "Project Management,Tech Lead,Leadership,Pre Sales";
    }

    public static class Messages
    {
        public const string NoDraftVersion = "No Draft version currently exists.";
        public const string NoPublishedVersion = "No Published version currently exists.";
        public const string TitleRequired = "Title is required.";
        public const string CategoryNotFound = "Selected category does not exist.";
        public const string DisplayOrderMinimum = "Display order must be at least 1.";
    }
}
