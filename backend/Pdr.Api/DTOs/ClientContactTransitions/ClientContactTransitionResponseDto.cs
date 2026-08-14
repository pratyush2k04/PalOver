namespace Pdr.Api.DTOs.ClientContactTransitions;

public class ClientContactTransitionResponseDto
{
    public int ClientContactTransitionId { get; set; }
    public int EngagementId { get; set; }
    public string OutgoingContactName { get; set; } = string.Empty;
    public string OutgoingContactRole { get; set; } = string.Empty;
    public string IncomingContactName { get; set; } = string.Empty;
    public string IncomingContactRole { get; set; } = string.Empty;
}
