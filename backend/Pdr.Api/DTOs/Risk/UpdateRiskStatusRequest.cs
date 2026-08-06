using Pdr.Api.Models.Risk;

namespace Pdr.Api.DTOs.Risk;

public class UpdateRiskStatusRequest
{
    public RiskStatus Status { get; set; }
}
