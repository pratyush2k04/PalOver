using System.Collections.Generic;
using System.Threading.Tasks;
using Pdr.Api.DTOs.Risks;

namespace Pdr.Api.Services.Interfaces;

public interface IBlockerEscalationService
{
    Task<int> ProcessEscalationsAsync();
    Task<List<RiskEscalationHistoryDto>> GetEscalationHistoryAsync(int riskId);
}
