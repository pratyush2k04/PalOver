using Pdr.Api.Models.Kickoff;

namespace Pdr.Api.DTOs.Kickoff;

public class UpdateActionItemStatusRequest
{
    public KickoffActionItemStatus Status { get; set; }
}
