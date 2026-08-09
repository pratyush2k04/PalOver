namespace Pdr.Api.Constants;

public static class AuthConstants
{
    public const int MaxFailedAttempts = 5;
    public const int LockoutDurationMinutes = 15;
    public const int DefaultExpiryMinutes = 30;

    public static class ConfigKeys
    {
        public const string SecretKey = "JwtSettings:SecretKey";
        public const string Issuer = "JwtSettings:Issuer";
        public const string Audience = "JwtSettings:Audience";
        public const string ExpiryMinutes = "JwtSettings:ExpiryMinutes";
    }

    public static class Messages
    {
        public const string InvalidCredentials = "Your email or password is incorrect.";
        public const string AccountLockedOut = "Too many attempts. Try again in 15 minutes or ask your Admin to reset your password.";
        public const string SecretKeyMissing = "JwtSettings:SecretKey is not configured.";
        public const string UnauthenticatedUser = "No authenticated user found on the current request.";
    }
}
