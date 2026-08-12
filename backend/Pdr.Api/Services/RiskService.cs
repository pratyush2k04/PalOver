using Pdr.Api.Constants;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RiskService(
    IRiskRepository riskRepository,
    ICurrentUserService currentUserService) : IRiskService
{
    public async Task<List<RiskDto>> GetRisksByEngagementAsync(int engagementId, RiskStatus? status = null, int? ownerId = null)
    {
        var risks = await riskRepository.GetByEngagementIdAsync(engagementId, status, ownerId);
        return risks.Select(MapToDto).ToList();
    }

    public async Task<RiskDto> CreateRiskAsync(int engagementId, CreateRiskDto dto)
    {
        var risk = new Risk
        {
            EngagementId = engagementId,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Impact = dto.Impact,
            Likelihood = dto.Likelihood,
            OwnerId = dto.OwnerId,
            Mitigation = string.IsNullOrWhiteSpace(dto.Mitigation) ? null : dto.Mitigation.Trim(),
            Status = RiskStatus.Open
        };

        await riskRepository.AddAsync(risk);
        await riskRepository.SaveChangesAsync();

        var created = await riskRepository.GetByIdAsync(risk.Id) ?? risk;
        return MapToDto(created);
    }

    public async Task<RiskDto> UpdateRiskAsync(int engagementId, int riskId, UpdateRiskDto dto)
    {
        var risk = await riskRepository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        if (risk.EngagementId != engagementId)
        {
            throw new RiskNotFoundException(riskId);
        }

        risk.Title = dto.Title.Trim();
        risk.Description = dto.Description.Trim();
        risk.Impact = dto.Impact;
        risk.Likelihood = dto.Likelihood;
        risk.OwnerId = dto.OwnerId;
        risk.Mitigation = string.IsNullOrWhiteSpace(dto.Mitigation) ? null : dto.Mitigation.Trim();

        // If status requested is Open or Mitigating, allow transition.
        // Closed status must go through CloseRiskAsync
        if (dto.Status == RiskStatus.Open || dto.Status == RiskStatus.Mitigating)
        {
            risk.Status = dto.Status;
        }

        await riskRepository.UpdateAsync(risk);
        await riskRepository.SaveChangesAsync();

        var updated = await riskRepository.GetByIdAsync(risk.Id) ?? risk;
        return MapToDto(updated);
    }

    public async Task<RiskDto> CloseRiskAsync(int engagementId, int riskId, CloseRiskDto dto)
    {
        var risk = await riskRepository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        if (risk.EngagementId != engagementId)
        {
            throw new RiskNotFoundException(riskId);
        }

        if (string.IsNullOrWhiteSpace(dto.ResolutionNote))
        {
            throw new ResolutionNoteRequiredException();
        }

        // Permission Check: Anyone can create/update, but ONLY PM or Owner can close
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement) || currentUserService.IsAdmin();
        var currentUserId = currentUserService.GetUserId();
        
        // Owner check: PM/Admin, or current logged-in user matches owner
        var isOwner = risk.OwnerId.HasValue && risk.OwnerId.Value == currentUserId;

        if (!isPm && !isOwner)
        {
            throw new RiskCloseAccessDeniedException();
        }

        risk.Status = RiskStatus.Closed;
        risk.ResolutionNote = dto.ResolutionNote.Trim();

        await riskRepository.UpdateAsync(risk);
        await riskRepository.SaveChangesAsync();

        var closed = await riskRepository.GetByIdAsync(risk.Id) ?? risk;
        return MapToDto(closed);
    }

    public async Task<RiskDto> ReopenRiskAsync(int engagementId, int riskId)
    {
        var risk = await riskRepository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        if (risk.EngagementId != engagementId)
        {
            throw new RiskNotFoundException(riskId);
        }

        risk.Status = RiskStatus.Open;
        risk.ResolutionNote = null;

        await riskRepository.UpdateAsync(risk);
        await riskRepository.SaveChangesAsync();

        var reopened = await riskRepository.GetByIdAsync(risk.Id) ?? risk;
        return MapToDto(reopened);
    }

    public async Task AutoCreateRiskFromBlockerAsync(
        int engagementId, 
        string title, 
        string blockReason, 
        int? ownerId, 
        string sourceType, 
        int sourceItemId, 
        string sourceTitle)
    {
        var risk = new Risk
        {
            EngagementId = engagementId,
            Title = RiskConstants.Messages.AutoCreatedPrefix(title),
            Description = blockReason.Trim(),
            Impact = RiskImpact.Medium,
            Likelihood = RiskLikelihood.Medium,
            OwnerId = ownerId,
            Mitigation = null,
            Status = RiskStatus.Open,
            SourceType = sourceType,
            SourceItemId = sourceItemId,
            SourceTitle = sourceTitle.Trim()
        };

        await riskRepository.AddAsync(risk);
        await riskRepository.SaveChangesAsync();
    }

    private static RiskDto MapToDto(Risk risk)
    {
        var openForDays = (DateTime.UtcNow - risk.CreatedAt).Days;

        return new RiskDto
        {
            Id = risk.Id,
            Title = risk.Title,
            Description = risk.Description,
            Impact = risk.Impact.ToString(),
            Likelihood = risk.Likelihood.ToString(),
            OwnerId = risk.OwnerId,
            OwnerName = risk.Owner?.FullName,
            Mitigation = risk.Mitigation,
            Status = risk.Status.ToString(),
            ResolutionNote = risk.ResolutionNote,
            OpenForDays = openForDays,
            SourceType = risk.SourceType,
            SourceItemId = risk.SourceItemId,
            SourceTitle = risk.SourceTitle
        };
    }
}
