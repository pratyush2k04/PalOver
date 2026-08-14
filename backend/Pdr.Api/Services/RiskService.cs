using Microsoft.EntityFrameworkCore;
using Pdr.Api.Constants;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RiskService(
    IRiskRepository riskRepository,
    AppDbContext dbContext,
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

        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement) || currentUserService.IsAdmin();
        var currentUserId = currentUserService.GetUserId();
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

    public async Task SyncAutoCreatedRisksAsync(int engagementId)
    {
        // 1. Fetch active Blocked items from AccessTracker for this engagement
        var blockedAccessItems = await dbContext.AccessItems
            .Where(a => a.EngagementId == engagementId &&
                        a.Status == AccessItemStatus.Blocked &&
                        !a.IsDeleted)
            .Select(a => new
            {
                SourceType = RiskConstants.SourceTypes.AccessTracker,
                SourceItemId = a.Id,
                Title = a.Name,
                Reason = a.BlockReason ?? string.Empty,
                OwnerId = a.OwnerId
            })
            .ToListAsync();

        // 2. Fetch active NotApplicable items from EnvironmentTooling for this engagement
        var notApplicableToolingItems = await dbContext.EnvironmentToolingItems
            .Include(e => e.EnvironmentToolingDefinition)
            .Where(e => e.EngagementId == engagementId &&
                        e.Status == EnvironmentToolingStatus.NotApplicable &&
                        !e.IsDeleted)
            .Select(e => new
            {
                SourceType = RiskConstants.SourceTypes.EnvironmentTooling,
                SourceItemId = e.EnvironmentToolingItemId,
                Title = e.EnvironmentToolingDefinition != null ? e.EnvironmentToolingDefinition.Name : (e.CustomName ?? e.Name),
                Reason = e.NotApplicableReason ?? string.Empty,
                OwnerId = e.OwnerId
            })
            .ToListAsync();

        // 3. Combine into a list of current active blockers
        var currentBlockers = blockedAccessItems.Concat(notApplicableToolingItems).ToList();

        // 4. Fetch all existing auto-created risks for this engagement
        var existingAutoRisks = await dbContext.Risks
            .Where(r => r.EngagementId == engagementId &&
                        r.SourceType != null &&
                        r.SourceItemId != null)
            .ToListAsync();

        // 5. Reconcile:
        // A) Create or update active blockers in risks table
        foreach (var blocker in currentBlockers)
        {
            var existing = existingAutoRisks.FirstOrDefault(r =>
                r.SourceType == blocker.SourceType &&
                r.SourceItemId == blocker.SourceItemId);

            if (existing == null)
            {
                var newRisk = new Risk
                {
                    EngagementId = engagementId,
                    Title = blocker.Title.Trim(),
                    Description = blocker.Reason.Trim(),
                    Impact = RiskImpact.Medium,
                    Likelihood = RiskLikelihood.Medium,
                    OwnerId = blocker.OwnerId,
                    Mitigation = null,
                    Status = RiskStatus.Open,
                    SourceType = blocker.SourceType,
                    SourceItemId = blocker.SourceItemId,
                    SourceTitle = blocker.Title.Trim()
                };

                await dbContext.Risks.AddAsync(newRisk);
            }
            else
            {
                existing.Title = blocker.Title.Trim();
                existing.Description = blocker.Reason.Trim();
                existing.OwnerId = blocker.OwnerId;
                existing.Status = RiskStatus.Open;
                existing.IsDeleted = false;
                existing.SourceTitle = blocker.Title.Trim();
            }
        }

        // B) Delete auto-created risks whose source item is NO LONGER blocked / not-applicable
        foreach (var existing in existingAutoRisks)
        {
            bool isStillBlocked = currentBlockers.Any(b =>
                b.SourceType == existing.SourceType &&
                b.SourceItemId == existing.SourceItemId);

            if (!isStillBlocked)
            {
                existing.IsDeleted = true;
            }
        }

        await dbContext.SaveChangesAsync();
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
