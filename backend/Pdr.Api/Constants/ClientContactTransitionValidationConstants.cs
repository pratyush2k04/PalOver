namespace Pdr.Api.Constants;

public static class ClientContactTransitionValidationConstants
{
    public const int MaxNameLength = 200;
    public const int MaxRoleLength = 200;

    public const string OutgoingContactNameRequired = "Outgoing contact name is required.";
    public const string OutgoingContactRoleRequired = "Outgoing contact role is required.";
    public const string IncomingContactNameRequired = "Incoming contact name is required.";
    public const string IncomingContactRoleRequired = "Incoming contact role is required.";
    public const string NameMaxLength = "Contact name cannot exceed 200 characters.";
    public const string RoleMaxLength = "Contact role cannot exceed 200 characters.";

    public static class Messages
    {
        public static string NotFound(int engagementId) => $"No client contact transition was found for engagement {engagementId}.";
    }
}
