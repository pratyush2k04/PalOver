namespace Pdr.Api.Constants;

public static class AssumptionConstants
{
    public static class Messages
    {
        public static string AssumptionNotFound(int id) => $"Assumption with ID {id} was not found.";
        public static string AlreadyValidated(int id) => $"Assumption with ID {id} has already been validated and cannot be edited.";
        public const string InvalidValidationStatus = "Cannot validate to 'Unvalidated' status.";
        public const string Unauthorized = "You are not authorized to modify this assumption.";
    }
}
