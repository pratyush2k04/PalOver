namespace Pdr.Api.Constants;

public static class DocumentValidationConstants
{
    public const string UploadsFolder = "Uploads/Documents";
    public const string DefaultContentType = "application/octet-stream";

    public const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB
    public static readonly string[] AllowedExtensions = { ".pdf", ".docx", ".xlsx", ".pptx", ".png", ".jpg", ".msg", ".eml" };

    public const int MinRemovalReasonLength = 10;
    public const int MaxRemovalReasonLength = 500;

    public static class Messages
    {
        public const string FileRequired = "File is required.";
        public const string FileCannotBeEmpty = "File cannot be empty.";
        public const string FileTooLarge = "Files must be 50 MB or smaller.";
        public const string FileTypeNotSupported = "That file type is not supported.";
        public const string EngagementIdRequired = "EngagementId must be greater than 0.";
        public const string CategoryIdRequired = "A valid CategoryId is required.";
        public const string VisibilityInvalid = "Visibility must be either 'PM-only' or 'Team-wide'.";

        public const string RemovalReasonRequired = "Reason for document removal is required.";
        public const string RemovalReasonMinLength = "Reason must be at least 10 characters.";
        public const string RemovalReasonMaxLength = "Reason cannot exceed 500 characters.";

        public const string VisibilityRequired = "Visibility is required.";
        public const string UpdateVisibilityInvalid = "Visibility must be 'Team-wide', 'PM-only', or 'Custom'.";
        public const string CustomVisibilityRolesRequired = "Allowed roles must be provided when setting custom visibility.";
        public const string CustomVisibilityAtLeastOneRole = "At least one role must be selected for custom visibility.";
        public const string InvalidRoleIds = "One or more role IDs are invalid.";

        public const string DownloadDenied = "You do not have permission to download this document.";
        public const string RemoveDenied = "You do not have permission to remove this document. Only the uploader or a Project Manager can remove it.";
        public const string SignedSowRemoveDenied = "The signed SOW cannot be removed because a submitted handover is relying on it.";
        public const string VisibilityChangeDenied = "You do not have permission to change document visibility. Only Project Managers and Admins can do this.";

        public static string NotFound(int id) => $"Document with id {id} was not found or already deleted.";
    }
}
