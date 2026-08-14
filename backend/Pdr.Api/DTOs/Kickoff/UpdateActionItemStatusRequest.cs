using Pdr.Api.Models;

namespace Pdr.Api.DTOs.Kickoff;

public class UpdateActionItemStatusRequest
{
    public KickoffActionItemStatus Status { get; set; }
}
