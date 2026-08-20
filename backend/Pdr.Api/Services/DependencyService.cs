using Microsoft.EntityFrameworkCore;
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
    IAuditLogService auditLogService) : IDependencyService
{
    public async Task<IEnumerable<DependencyDto>> GetDependenciesAsync(int engagementId, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var dependencies = (await dependencyRepository.GetByEngagementIdAsync(engagementId)).ToList();
        
        bool hasChanges = false;
        var today = DateTime.UtcNow.Date;

        foreach (var dep in dependencies)
        {
            if (dep.Status == DependencyStatus.Open && dep.NeededByDate.Date < today)
            {
                dep.Status = DependencyStatus.AtRisk;
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

    public async Task<DependencyDto> CreateDependencyAsync(int engagementId, CreateDependencyDto dto, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

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

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyCreated,
            new Dictionary<string, object?> { { "DependencyId", dependency.Id }, { "Type", dependency.Type.ToString() } },
            engagementId,
            ParseUserId(currentUserId)
        );

        var savedDependency = await dependencyRepository.GetByIdAsync(dependency.Id);
        return MapToDto(savedDependency!);
    }

    public async Task<DependencyDto> UpdateDependencyAsync(int engagementId, int id, UpdateDependencyDto dto, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var dependency = await dependencyRepository.GetByIdAsync(id) 
            ?? throw new DependencyNotFoundException(id);

        if (dependency.EngagementId != engagementId)
            throw new UnauthorizedAccessException();

        dependency.Description = dto.Description;
        dependency.Type = dto.Type;
        dependency.OwnerId = dto.OwnerId;
        dependency.NeededByDate = dto.NeededByDate;
        
        // Handle marking as met
        if (dependency.Status != DependencyStatus.Met && dto.Status == DependencyStatus.Met)
        {
            if (dependency.LinkedRiskId.HasValue)
            {
                var risk = await riskRepository.GetByIdAsync(dependency.LinkedRiskId.Value);
                if (risk != null && risk.Status != RiskStatus.Closed)
                {
                    risk.Status = RiskStatus.Closed;
                    risk.ResolutionNote = "Dependency met.";
                    await riskRepository.UpdateAsync(risk);
                    // Assume risk changes are audited in a generic way or omitted here for brevity
                }
            }
        }
        
        dependency.Status = dto.Status;

        if (dependency.Status == DependencyStatus.Open && dependency.NeededByDate.Date < DateTime.UtcNow.Date)
        {
            dependency.Status = DependencyStatus.AtRisk;
        }

        dependencyRepository.Update(dependency);
        await dependencyRepository.SaveChangesAsync();

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyUpdated,
            new Dictionary<string, object?> { { "DependencyId", dependency.Id }, { "Status", dependency.Status.ToString() } },
            engagementId,
            ParseUserId(currentUserId)
        );

        var savedDependency = await dependencyRepository.GetByIdAsync(dependency.Id);
        return MapToDto(savedDependency!);
    }

    public async Task DeleteDependencyAsync(int engagementId, int id, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var dependency = await dependencyRepository.GetByIdAsync(id) 
            ?? throw new DependencyNotFoundException(id);

        if (dependency.EngagementId != engagementId)
            throw new UnauthorizedAccessException();

        dependencyRepository.Delete(dependency);
        await dependencyRepository.SaveChangesAsync();

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyDeleted,
            new Dictionary<string, object?> { { "DependencyId", id } },
            engagementId,
            ParseUserId(currentUserId)
        );
    }

    public async Task<DependencyDto> EscalateToRiskAsync(int engagementId, int id, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var dependency = await dependencyRepository.GetByIdAsync(id) 
            ?? throw new DependencyNotFoundException(id);

        if (dependency.EngagementId != engagementId)
            throw new UnauthorizedAccessException();

        if (dependency.Type != DependencyType.External)
            throw new DependencyValidationException("Only external dependencies can be escalated.");

        if (dependency.Status != DependencyStatus.AtRisk)
            throw new DependencyValidationException("Only At Risk dependencies can be escalated.");

        if (dependency.LinkedRiskId.HasValue)
            throw new DependencyValidationException("Dependency is already escalated.");

        var risk = new Risk
        {
            EngagementId = engagementId,
            Title = $"Escalated Dependency: {dependency.Description.Substring(0, Math.Min(50, dependency.Description.Length))}",
            Description = $"This risk was auto-created from an overdue external dependency.\n\nDescription: {dependency.Description}",
            Impact = RiskImpact.Medium,
            Likelihood = RiskLikelihood.Medium,
            Status = RiskStatus.Open
        };

        await riskRepository.AddAsync(risk);
        await riskRepository.SaveChangesAsync(); // Save to generate ID

        dependency.LinkedRiskId = risk.Id;
        dependencyRepository.Update(dependency);
        await dependencyRepository.SaveChangesAsync();

        await auditLogService.LogAsync(
            AuditActionEnum.DependencyEscalated,
            new Dictionary<string, object?> { { "DependencyId", dependency.Id }, { "RiskId", risk.Id } },
            engagementId,
            ParseUserId(currentUserId)
        );

        return MapToDto(dependency);
    }

    private async Task ValidateUserAccessAsync(int engagementId, string? currentUserId)
    {
        if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var userId))
            throw new UnauthorizedAccessException();

        var assignment = await engagementRepository.GetAssignmentByUserAsync(engagementId, userId);
        if (assignment == null)
            throw new UnauthorizedAccessException("User does not have access to this engagement.");
    }

    private int? ParseUserId(string? currentUserId)
    {
        return int.TryParse(currentUserId, out var id) ? id : null;
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
