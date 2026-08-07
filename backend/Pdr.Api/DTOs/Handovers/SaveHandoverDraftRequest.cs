namespace Pdr.Api.DTOs.Handovers;

public class SaveHandoverDraftRequest
{
    public string? ClientBackground { get; set; }
    public string? BusinessProblem { get; set; }
    public string? ClientSentiment { get; set; }
    public string? DealOrigin { get; set; }
}
