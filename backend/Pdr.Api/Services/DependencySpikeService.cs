using Pdr.Api.DTOs.DependencySpikes;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.DependencySpikes;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class DependencySpikeService(
    IDependencySpikeRepository repository,
    IEngagementRepository engagementRepository,
    ITechnicalReadinessNotificationService readinessNotificationService) : IDependencySpikeService
{
    public async Task<List<DependencySpikeDto>> GetSpikesByEngagementIdAsync(int engagementId)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var spikes = await repository.GetByEngagementIdAsync(engagementId);
        return spikes.Select(MapToDto).ToList();
    }

    public async Task<DependencySpikeDto> CreateSpikeAsync(int engagementId, CreateDependencySpikeRequest request)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        int? ownerId = request.OwnerId;

        // Auto-assign Tech Lead if ownerId not provided
        if (!ownerId.HasValue)
        {
            var techLeadAssignment = await engagementRepository.GetAssignmentByRoleIdAsync(engagementId, (int)RoleEnum.TechLead);
            ownerId = techLeadAssignment?.UserId;
        }

        var wasReady = await readinessNotificationService.IsReadyAsync(engagementId);

        var spike = new DependencySpike
        {
            EngagementId = engagementId,
            DependencyName = request.DependencyName,
            Objective = request.Objective,
            Status = request.Status,
            OwnerId = ownerId,
            OutcomeFindings = request.OutcomeFindings
        };

        await repository.AddAsync(spike);
        await repository.SaveChangesAsync();

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(engagementId, wasReady);

        var created = await repository.GetByIdAsync(spike.Id);
        return MapToDto(created!);
    }

    public async Task<DependencySpikeDto> UpdateSpikeAsync(int spikeId, UpdateDependencySpikeRequest request)
    {
        var spike = await repository.GetByIdAsync(spikeId);
        if (spike is null)
        {
            throw new DependencySpikeNotFoundException(spikeId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(spike.EngagementId);

        var wasReady = await readinessNotificationService.IsReadyAsync(spike.EngagementId);

        spike.DependencyName = request.DependencyName;
        spike.Objective = request.Objective;
        spike.Status = request.Status;
        spike.OwnerId = request.OwnerId;
        spike.OutcomeFindings = request.OutcomeFindings;

        await repository.SaveChangesAsync();

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(spike.EngagementId, wasReady);

        var updated = await repository.GetByIdAsync(spikeId);
        return MapToDto(updated!);
    }

    public async Task DeleteSpikeAsync(int spikeId)
    {
        var spike = await repository.GetByIdAsync(spikeId);
        if (spike is null)
        {
            throw new DependencySpikeNotFoundException(spikeId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(spike.EngagementId);

        var wasReady = await readinessNotificationService.IsReadyAsync(spike.EngagementId);

        spike.IsDeleted = true;
        await repository.SaveChangesAsync();

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(spike.EngagementId, wasReady);
    }

    private static DependencySpikeDto MapToDto(DependencySpike spike) =>
        new()
        {
            Id = spike.Id,
            EngagementId = spike.EngagementId,
            DependencyName = spike.DependencyName,
            Objective = spike.Objective,
            Status = spike.Status,
            OwnerId = spike.OwnerId,
            OwnerName = spike.Owner?.FullName,
            OutcomeFindings = spike.OutcomeFindings,
            CreatedAt = spike.CreatedAt,
            UpdatedAt = spike.UpdatedAt
        };
}
