using Pdr.Api.Constants;
using Pdr.Api.DTOs.ArchitectureValidation;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.ArchitectureValidation;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ArchitectureValidationService(
    IEngagementRepository engagementRepository,
    IArchitectureValidationRepository repository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService,
    ITechnicalReadinessNotificationService readinessNotificationService,
    IEngagementAccessService engagementAccessService) : IArchitectureValidationService
{
    public async Task<ArchitectureValidationDto> GetByEngagementIdAsync(int engagementId)
    {
        await engagementAccessService.ValidateAccessAsync(engagementId);

        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var validation = await repository.GetByEngagementIdAsync(engagementId);
        if (validation is null)
        {
            validation = new ArchitectureValidation
            {
                EngagementId = engagementId,
                Status = ArchitectureStatus.StillValid
            };
            await repository.AddValidationAsync(validation);
            await repository.SaveChangesAsync();
        }

        var committedTimelineWeeks = await repository.GetCommittedTimelineWeeksAsync(engagementId);

        return BuildDto(validation, engagementId, committedTimelineWeeks);
    }

    public async Task<ArchitectureValidationDto> SaveValidationAsync(int engagementId, SaveArchitectureValidationRequest request)
    {
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.TechLead);

        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        if (request.Status == ArchitectureStatus.Modified && string.IsNullOrWhiteSpace(request.Rationale))
        {
            throw new RationaleRequiredWhenArchitectureModifiedException();
        }

        var wasReady = await readinessNotificationService.IsReadyAsync(engagementId);

        var validation = await repository.GetByEngagementIdAsync(engagementId);
        if (validation is null)
        {
            validation = new ArchitectureValidation
            {
                EngagementId = engagementId,
                Status = request.Status,
                Rationale = request.Rationale?.Trim()
            };
            await repository.AddValidationAsync(validation);
        }
        else
        {
            validation.Status = request.Status;
            validation.Rationale = request.Rationale?.Trim();
        }

        await repository.SaveChangesAsync();

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(engagementId, wasReady);

        var committedTimelineWeeks = await repository.GetCommittedTimelineWeeksAsync(engagementId);
        return BuildDto(validation, engagementId, committedTimelineWeeks);
    }

    public async Task<DiscoveryFeatureItemDto> AddFeatureItemAsync(int engagementId, CreateDiscoveryFeatureItemRequest request)
    {
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.TechLead);

        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var validation = await repository.GetByEngagementIdAsync(engagementId);
        if (validation is null)
        {
            validation = new ArchitectureValidation
            {
                EngagementId = engagementId,
                Status = ArchitectureStatus.StillValid
            };
            await repository.AddValidationAsync(validation);
            await repository.SaveChangesAsync();
        }

        var wasReady = await readinessNotificationService.IsReadyAsync(engagementId);

        var item = new DiscoveryFeatureItem
        {
            ArchitectureValidationId = validation.Id,
            FeatureName = request.FeatureName.Trim(),
            EstimateWeeks = request.EstimateWeeks,
            TargetSprint = request.TargetSprint.Trim()
        };

        await repository.AddFeatureItemAsync(item);
        await repository.SaveChangesAsync();

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.DiscoveryItemAdded,
            new Dictionary<string, object?>
            {
                ["EngagementId"] = engagementId,
                ["ItemName"] = item.FeatureName,
                ["ActorUserId"] = actorUserId
            },
            engagementId: engagementId,
            actingUserId: actorUserId > 0 ? actorUserId : null);

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(engagementId, wasReady);

        return MapFeatureToDto(item);
    }

    public async Task<DiscoveryFeatureItemDto> UpdateFeatureItemAsync(int featureId, UpdateDiscoveryFeatureItemRequest request)
    {
        var item = await repository.GetFeatureItemByIdAsync(featureId);
        if (item is null)
        {
            throw new DiscoveryFeatureItemNotFoundException(featureId);
        }

        var engagementId = item.ArchitectureValidation?.EngagementId ?? 0;
        if (engagementId > 0)
        {
            await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.TechLead);
            await engagementRepository.EnsureNotSignedOffAsync(engagementId);
        }

        var wasReady = engagementId > 0 && await readinessNotificationService.IsReadyAsync(engagementId);

        item.FeatureName = request.FeatureName.Trim();
        item.EstimateWeeks = request.EstimateWeeks;
        item.TargetSprint = request.TargetSprint.Trim();

        await repository.SaveChangesAsync();

        if (engagementId > 0)
        {
            await readinessNotificationService.CheckAndNotifyIfReadyAsync(engagementId, wasReady);
        }

        return MapFeatureToDto(item);
    }

    public async Task DeleteFeatureItemAsync(int featureId)
    {
        var item = await repository.GetFeatureItemByIdAsync(featureId);
        if (item is null)
        {
            throw new DiscoveryFeatureItemNotFoundException(featureId);
        }

        var engagementId = item.ArchitectureValidation?.EngagementId ?? 0;
        if (engagementId > 0)
        {
            await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.TechLead);
            await engagementRepository.EnsureNotSignedOffAsync(engagementId);
        }

        var wasReady = engagementId > 0 && await readinessNotificationService.IsReadyAsync(engagementId);

        item.IsDeleted = true;
        await repository.SaveChangesAsync();

        if (engagementId > 0)
        {
            await readinessNotificationService.CheckAndNotifyIfReadyAsync(engagementId, wasReady);
        }
    }

    private static ArchitectureValidationDto BuildDto(ArchitectureValidation? validation, int engagementId, decimal committedTimelineWeeks)
    {
        var activeFeatures = validation?.Features.Where(f => !f.IsDeleted).ToList() ?? [];
        var totalEstimatedWeeks = activeFeatures.Sum(f => f.EstimateWeeks);
        var sizingDeltaWeeks = totalEstimatedWeeks - committedTimelineWeeks;
        var sizingDeltaPercentage = committedTimelineWeeks > 0
            ? Math.Round((sizingDeltaWeeks / committedTimelineWeeks) * 100m, 1)
            : 0m;

        var isRiskRaised = committedTimelineWeeks > 0 && sizingDeltaPercentage > ArchitectureValidationConstants.SizingRiskThresholdPercentage;
        string? riskSummary = null;

        if (isRiskRaised)
        {
            riskSummary = $"Discovery estimate ({totalEstimatedWeeks:0.#} wks) exceeds committed timeline ({committedTimelineWeeks:0.#} wks) by {sizingDeltaPercentage:0.#}%. Technical readiness criteria cannot be met until scope or timeline is adjusted.";
        }

        return new ArchitectureValidationDto
        {
            Id = validation?.Id ?? 0,
            EngagementId = engagementId,
            Status = validation?.Status ?? ArchitectureStatus.StillValid,
            Rationale = validation?.Rationale,
            CommittedTimelineWeeks = committedTimelineWeeks,
            TotalEstimatedWeeks = totalEstimatedWeeks,
            SizingDeltaWeeks = sizingDeltaWeeks,
            SizingDeltaPercentage = sizingDeltaPercentage,
            IsRiskRaised = isRiskRaised,
            RiskSummary = riskSummary,
            Features = activeFeatures.Select(MapFeatureToDto).ToList(),
            UpdatedAt = validation?.UpdatedAt ?? validation?.CreatedAt ?? DateTime.UtcNow,
            UpdatedBy = validation?.UpdatedBy ?? validation?.CreatedBy
        };
    }

    private static DiscoveryFeatureItemDto MapFeatureToDto(DiscoveryFeatureItem item) =>
        new()
        {
            Id = item.Id,
            ArchitectureValidationId = item.ArchitectureValidationId,
            FeatureName = item.FeatureName,
            EstimateWeeks = item.EstimateWeeks,
            TargetSprint = item.TargetSprint,
            CreatedAt = item.CreatedAt,
            CreatedBy = item.CreatedBy
        };
}
