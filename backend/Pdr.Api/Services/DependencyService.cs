using Microsoft.EntityFrameworkCore;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.DTOs.Dependencies;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.Engagements.Dependencies;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class DependencyService(
    IDependencyRepository dependencyRepository,
    IEngagementRepository engagementRepository,
    IRiskRepository riskRepository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : IDependencyService
{
    public async Task<IEnumerable<DependencyDto>> GetDependenciesAsync(int engagementId)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var dependencies = (await dependencyRepository.GetByEngagementIdAsync(engagementId)).ToList();
        
        bool hasChanges = false;
        var today = DateTime.UtcNow.Date;

        foreach (var dep in dependencies)
        {
            if (dep.Status == DependencyStatus.Open && dep.NeededByDate.Date < today)
            {
                dep.Status = DependencyStatus.AtRisk;
                await CreateRiskForDependencyAsync(dep, engagementId);
                dependencyRepository.Update(dep);
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await dependencyRepository.SaveChangesAsync();
        }

        return dependencies.Select(MapToDto);
    }

    public async Task<DependencyDto> CreateDependencyAsync(int engagementId, CreateDependencyDto dto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var dependency = new Dependency
        {
            EngagementId = engagementId,
            Description = dto.Description,
            Type = dto.Type,
            OwnerId = dto.OwnerId,
            NeededByDate = dto.NeededByDate,
            Status = DependencyStatus.Open
        };

        if (dependency.NeededByDate.Date < DateTime.UtcNow.Date)
        {
            dependency.Status = DependencyStatus.AtRisk;
        }

        await dependencyRepository.AddAsync(dependency);
        await dependencyRepository.SaveChangesAsync();

        if (dependency.Status == DependencyStatus.AtRisk)
        {
            await CreateRiskForDependencyAsync(dependency, engagementId);
            dependencyRepository.Update(dependency);
            await dependencyRepository.SaveChangesAsync();
        }

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyCreated,
            new Dictionary<string, object?> { { "DependencyId", dependency.Id }, { "Type", dependency.Type.ToString() } },
            engagementId,
            currentUserService.GetUserId()
        );

        var savedDependency = await dependencyRepository.GetByIdAsync(dependency.Id);
        return MapToDto(savedDependency!);
    }

    public async Task<DependencyDto> UpdateDependencyAsync(int engagementId, int id, UpdateDependencyDto dto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var dependency = await dependencyRepository.GetByIdAsync(id);
        if (dependency == null || dependency.EngagementId != engagementId)
            throw new DependencyNotFoundException(id);

        var currentUserId = currentUserService.GetUserId();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);

        if (dependency.Owner?.UserId != currentUserId && !isPm)
            throw new DependencyUnauthorizedException();

        dependency.Description = dto.Description;
        dependency.Type = dto.Type;
        dependency.OwnerId = dto.OwnerId;
        dependency.NeededByDate = dto.NeededByDate;
        
        bool isBeingMarkedAsMet = (dependency.Status != DependencyStatus.Met) && (dto.Status == DependencyStatus.Met);

        if (isBeingMarkedAsMet && dependency.LinkedRiskId.HasValue)
        {
            await CloseLinkedRiskAsync(dependency.LinkedRiskId.Value);
        }
        
        dependency.Status = dto.Status;

        if (dependency.Status == DependencyStatus.Open && dependency.NeededByDate.Date < DateTime.UtcNow.Date)
        {
            dependency.Status = DependencyStatus.AtRisk;
        }

        if (dependency.Status == DependencyStatus.AtRisk)
        {
            await CreateRiskForDependencyAsync(dependency, engagementId);
        }

        dependencyRepository.Update(dependency);
        await dependencyRepository.SaveChangesAsync();

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyUpdated,
            new Dictionary<string, object?> { { "DependencyId", dependency.Id }, { "Status", dependency.Status.ToString() } },
            engagementId,
            currentUserService.GetUserId()
        );

        var savedDependency = await dependencyRepository.GetByIdAsync(dependency.Id);
        return MapToDto(savedDependency!);
    }

    public async Task DeleteDependencyAsync(int engagementId, int id)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var dependency = await dependencyRepository.GetByIdAsync(id);
        if (dependency == null || dependency.EngagementId != engagementId)
            throw new DependencyNotFoundException(id);

        var currentUserId = currentUserService.GetUserId();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);

        if (dependency.OwnerId != currentUserId && !isPm)
            throw new DependencyUnauthorizedException();

        dependencyRepository.Delete(dependency);
        await dependencyRepository.SaveChangesAsync();

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyDeleted,
            new Dictionary<string, object?> { { "DependencyId", id } },
            engagementId,
            currentUserService.GetUserId()
        );
    }

    private async Task CreateRiskForDependencyAsync(Dependency dependency, int engagementId)
    {
        if (dependency.LinkedRiskId.HasValue) return;

        var risk = new Risk
        {
            EngagementId = engagementId,
            Title = $"Escalated Dependency: {dependency.Description.Substring(0, Math.Min(50, dependency.Description.Length))}",
            Description = $"This risk was auto-created from an overdue dependency.\n\nDescription: {dependency.Description}",
            Impact = RiskImpact.Medium,
            Likelihood = RiskLikelihood.Medium,
            Status = RiskStatus.Open,
            OwnerId = dependency.Owner?.UserId
        };

        await riskRepository.AddAsync(risk);
        await riskRepository.SaveChangesAsync();

        dependency.LinkedRiskId = risk.Id;

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyEscalated,
            new Dictionary<string, object?> { { "DependencyId", dependency.Id }, { "RiskId", risk.Id } },
            engagementId,
            currentUserService.GetUserId()
        );
    }

    private async Task CloseLinkedRiskAsync(int riskId)
    {
        var risk = await riskRepository.GetByIdAsync(riskId);
        if (risk != null && risk.Status != RiskStatus.Closed)
        {
            risk.Status = RiskStatus.Closed;
            risk.ResolutionNote = "Dependency met.";
            await riskRepository.UpdateAsync(risk);
        }
    }

    private static DependencyDto MapToDto(Dependency dependency)
    {
        return new DependencyDto
        {
            Id = dependency.Id,
            EngagementId = dependency.EngagementId,
            Description = dependency.Description,
            Type = dependency.Type,
            OwnerId = dependency.OwnerId,
            OwnerName = dependency.Owner?.Name ?? string.Empty,
            NeededByDate = dependency.NeededByDate,
            Status = dependency.Status,
            LinkedRiskId = dependency.LinkedRiskId,
            CreatedAt = dependency.CreatedAt,
            UpdatedAt = dependency.UpdatedAt,
            CreatedBy = dependency.CreatedBy,
            UpdatedBy = dependency.UpdatedBy
        };
    }
}
