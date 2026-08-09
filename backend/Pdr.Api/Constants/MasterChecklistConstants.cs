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

    public static class StatusNames
    {
        public const string Draft = "Draft";
        public const string Published = "Published";
    }

    public static class ItemStatuses
    {
        public const string Incomplete = "Incomplete";
        public const string Complete = "Complete";
    }

    public static class ExemptionStatuses
    {
        public const string None = "None";
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }

    public static class Audit
    {
        public const string SystemUser = "System";
    }

    public static class Messages
    {
        public const string NoDraftVersion = "No Draft version currently exists.";
        public const string NoPublishedVersion = "No Published version currently exists.";
        public const string TitleRequired = "Title is required.";
        public const string CategoryNotFound = "Selected category does not exist.";
        public const string DisplayOrderMinimum = "Display order must be at least 1.";
        public const string ItemsRequired = "At least one item is required to reorder.";

        public const string VersionNotFound = "Checklist version not found.";
        public const string ItemNotFound = "Checklist item not found.";
        public const string NoDraftToAddItems = "No Draft version exists to add items to.";
        public const string OnlyDraftEditable = "Only items on the Draft version can be edited.";
        public const string OnlyDraftRetirable = "Only items on the Draft version can be retired.";
        public const string NoDraftToReorder = "No Draft version exists to reorder.";
        public const string ItemDoesNotBelongToDraft = "One or more items do not belong to the current Draft version.";
        public const string ReorderMustIncludeAllItems = "Reorder request must include every item in the current Draft version exactly once.";
        public const string DisplayOrderMustBeUnique = "Display order values must be unique.";
        public const string OnlyDraftPublishable = "Only a Draft version can be published.";
        public const string StatusesNotConfigured = "Checklist statuses are not configured correctly.";
        public const string FailedToSaveChecklist = "Failed to retrieve saved checklist.";
        public const string FailedToUpdateChecklist = "Failed to retrieve updated checklist.";
    }
}
