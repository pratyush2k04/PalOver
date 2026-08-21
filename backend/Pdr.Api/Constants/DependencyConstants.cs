namespace Pdr.Api.Constants;

public static class DependencyConstants
{
    public static class Messages
    {
        public static string DependencyNotFound(int id) => $"Dependency with ID {id} was not found.";
        public const string Unauthorized = "You do not have permission to modify this dependency.";
    }
}
