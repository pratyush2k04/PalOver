namespace Pdr.Api.DTOs.EnvironmentTooling
{
    public class AddCustomEnvironmentToolingItemRequest
    {
        public string CustomName { get; set; } = string.Empty;
        public int OwnerId { get; set; }
    }
}
