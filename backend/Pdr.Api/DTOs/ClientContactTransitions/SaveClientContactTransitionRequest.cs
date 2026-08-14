namespace Pdr.Api.DTOs.ClientContactTransitions;

public class SaveClientContactTransitionRequest
{
    public string OutgoingContactName { get; set; } = string.Empty;
    public string OutgoingContactRole { get; set; } = string.Empty;
    public string IncomingContactName { get; set; } = string.Empty;
    public string IncomingContactRole { get; set; } = string.Empty;
}
