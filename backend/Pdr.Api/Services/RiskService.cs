using Pdr.Api.DTOs.Risk;
using Pdr.Api.Exceptions;
using Pdr.Api.Exceptions.RiskExceptions;
using Pdr.Api.Models.Risk;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RiskService(IRiskRepository repository) : IRiskService
{
    public async Task CreateRiskAsync(int engagementId, string title, string description, int? accessItemId = null, int? environmentToolingItemId = null)
    {
        var existingOpenRisk = await repository.GetOpenRiskAsync(engagementId, title, accessItemId, environmentToolingItemId);
        if (existingOpenRisk != null)
        {
            return;
        }

        var risk = new Risk
        {
            EngagementId = engagementId,
            AccessItemId = accessItemId,
            EnvironmentToolingItemId = environmentToolingItemId,
            Title = title,
            Description = description,
            Status = RiskStatus.Open
        };

        await repository.AddAsync(risk);
        await repository.SaveChangesAsync();
    }

    public async Task ResolveRiskAsync(int engagementId, string title, int? accessItemId = null, int? environmentToolingItemId = null)
    {
        var openRisk = await repository.GetOpenRiskAsync(engagementId, title, accessItemId, environmentToolingItemId);
        if (openRisk != null)
        {
            openRisk.Status = RiskStatus.Closed;
            await repository.SaveChangesAsync();
        }
    }

    public async Task CreateClientSideRiskAsync(int engagementId, string accessItemName, int? accessItemId = null)
    {
        var title = $"Client Access Not Granted: {accessItemName}";
        var description = $"The client-side access item '{accessItemName}' has not been granted and billing is starting within 2 days.";
        await CreateRiskAsync(engagementId, title, description, accessItemId: accessItemId);
    }

    public async Task<List<RiskDto>> GetRisksByEngagementIdAsync(int engagementId)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var risks = await repository.GetByEngagementIdAsync(engagementId);
        return risks.Select(MapToDto).ToList();
    }

    public async Task<RiskDto> CreateManualRiskAsync(int engagementId, CreateRiskRequest request)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var risk = new Risk
        {
            EngagementId = engagementId,
            Title = request.Title,
            Description = request.Description,
            Status = RiskStatus.Open
        };

        await repository.AddAsync(risk);
        await repository.SaveChangesAsync();

        return MapToDto(risk);
    }

    public async Task<RiskDto> UpdateRiskStatusAsync(int riskId, UpdateRiskStatusRequest request)
    {
        var risk = await repository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        risk.Status = request.Status;
        await repository.SaveChangesAsync();

        return MapToDto(risk);
    }

    private static RiskDto MapToDto(Risk risk) => new()
    {
        RiskId = risk.RiskId,
        EngagementId = risk.EngagementId,
        AccessItemId = risk.AccessItemId,
        EnvironmentToolingItemId = risk.EnvironmentToolingItemId,
        Title = risk.Title,
        Description = risk.Description,
        Status = risk.Status,
        CreatedAt = risk.CreatedAt,
        UpdatedAt = risk.UpdatedAt,
        CreatedBy = risk.CreatedBy,
        UpdatedBy = risk.UpdatedBy
    };
}
