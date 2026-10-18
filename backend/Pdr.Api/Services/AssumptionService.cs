using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Assumptions;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Models.Audit;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class AssumptionService(
    IAssumptionRepository assumptionRepository,
    IEngagementRepository engagementRepository,
    ICurrentUserService currentUserService,
    INotificationService notificationService,
    IAuditLogService auditLogService) : IAssumptionService
{
    public async Task<List<AssumptionDto>> GetAssumptionsAsync(int engagementId)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var assumptions = await assumptionRepository.GetByEngagementIdAsync(engagementId);
        return assumptions
            .OrderByDescending(a => a.Status == AssumptionStatus.Unvalidated || a.Status == AssumptionStatus.Invalid)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<AssumptionDto> GetAssumptionAsync(int engagementId, int assumptionId)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var assumption = await assumptionRepository.GetByIdAsync(assumptionId);
        if (assumption == null || assumption.EngagementId != engagementId)
            throw new AssumptionNotFoundException(assumptionId);

        return MapToDto(assumption);
    }

    public async Task<AssumptionDto> CreateAssumptionAsync(int engagementId, CreateAssumptionDto createDto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        var isPm = assignments.Any(a => a.UserId == currentUserService.GetUserId() && a.EngagementRoleId == (int)EngagementRoleEnum.ProjectManager);

        var assumption = new Assumption
        {
            EngagementId = engagementId,
            Description = createDto.Description,
            Rationale = createDto.Rationale,
            OwnerId = currentUserService.GetUserId(),
            Status = isPm ? AssumptionStatus.Validated : AssumptionStatus.Unvalidated
        };

        var created = await assumptionRepository.AddAsync(assumption);

        if (created.Status == AssumptionStatus.Unvalidated)
        {
            var data = JsonSerializer.Serialize(
                new AssumptionNeedsValidationMapper.AssumptionNeedsValidationPayload(created.Id, engagementId));

            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.AssumptionNeedsValidation,
                engagementId,
                EngagementRoleEnum.ProjectManager,
                data,
                DateTime.UtcNow);
        }

        await auditLogService.LogAsync(
            AuditActionEnum.AssumptionCreated,
            new Dictionary<string, object?>
            {
                ["AssumptionId"] = created.Id
            },
            engagementId);

        return MapToDto(created);
    }

    public async Task<AssumptionDto> UpdateAssumptionAsync(int engagementId, int assumptionId, UpdateAssumptionDto updateDto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var assumption = await assumptionRepository.GetByIdAsync(assumptionId);
        if (assumption == null || assumption.EngagementId != engagementId)
            throw new AssumptionNotFoundException(assumptionId);

        var currentUserId = currentUserService.GetUserId();

        if (assumption.OwnerId != currentUserId)
            throw new AssumptionUnauthorizedException();

        assumption.Description = updateDto.Description;
        assumption.Rationale = updateDto.Rationale;

        var updated = await assumptionRepository.UpdateAsync(assumption);

        await auditLogService.LogAsync(
            AuditActionEnum.AssumptionUpdated,
            new Dictionary<string, object?>
            {
                ["AssumptionId"] = updated.Id
            },
            engagementId);

        return MapToDto(updated);
    }

    public async Task<AssumptionDto> ValidateAssumptionAsync(int engagementId, int assumptionId, ValidateAssumptionDto validateDto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var assumption = await assumptionRepository.GetByIdAsync(assumptionId);
        if (assumption == null || assumption.EngagementId != engagementId)
            throw new AssumptionNotFoundException(assumptionId);

        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        var isPm = assignments.Any(a => a.UserId == currentUserService.GetUserId() && a.EngagementRoleId == (int)EngagementRoleEnum.ProjectManager);

        if (!isPm)
            throw new AssumptionUnauthorizedException();

        if (assumption.Status != AssumptionStatus.Unvalidated)
            throw new AssumptionAlreadyValidatedException(assumptionId);

        if (validateDto.Status == AssumptionStatus.Unvalidated)
            throw new InvalidAssumptionStatusException();

        assumption.Status = validateDto.Status;

        var updated = await assumptionRepository.UpdateAsync(assumption);

        await auditLogService.LogAsync(
            AuditActionEnum.AssumptionValidated,
            new Dictionary<string, object?>
            {
                ["AssumptionId"] = updated.Id,
                ["Status"] = validateDto.Status.ToString()
            },
            engagementId);

        return MapToDto(updated);
    }

    private static AssumptionDto MapToDto(Assumption assumption)
    {
        return new AssumptionDto
        {
            Id = assumption.Id,
            EngagementId = assumption.EngagementId,
            Description = assumption.Description,
            Rationale = assumption.Rationale,
            OwnerId = assumption.OwnerId,
            OwnerName = assumption.Owner?.FullName ?? string.Empty,
            Status = assumption.Status
        };
    }
}
