using Pdr.Api.DTOs.Assumptions;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class AssumptionService(
    IAssumptionRepository assumptionRepository,
    IEngagementRepository engagementRepository,
    IRiskService riskService,
    ICurrentUserService currentUserService) : IAssumptionService
{
    public async Task<List<AssumptionDto>> GetAssumptionsAsync(int engagementId)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var assumptions = await assumptionRepository.GetByEngagementIdAsync(engagementId);
        return assumptions
            .OrderByDescending(a => a.Status == AssumptionStatus.Unvalidated || (a.Status == AssumptionStatus.Invalid && a.LinkedRiskId == null))
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

        var assumption = new Assumption
        {
            EngagementId = engagementId,
            Description = createDto.Description,
            Rationale = createDto.Rationale,
            OwnerId = currentUserService.GetUserId(),
            Status = AssumptionStatus.Unvalidated
        };

        var created = await assumptionRepository.AddAsync(assumption);

        return MapToDto(created);
    }

    public async Task<AssumptionDto> UpdateAssumptionAsync(int engagementId, int assumptionId, UpdateAssumptionDto updateDto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var assumption = await assumptionRepository.GetByIdAsync(assumptionId);
        if (assumption == null || assumption.EngagementId != engagementId)
            throw new AssumptionNotFoundException(assumptionId);

        var currentUserId = currentUserService.GetUserId();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);

        if (assumption.OwnerId != currentUserId && !isPm)
            throw new AssumptionUnauthorizedException();

        assumption.Description = updateDto.Description;
        assumption.Rationale = updateDto.Rationale;

        var updated = await assumptionRepository.UpdateAsync(assumption);

        return MapToDto(updated);
    }

    public async Task<AssumptionDto> ValidateAssumptionAsync(int engagementId, int assumptionId, ValidateAssumptionDto validateDto)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId);
        if (engagement == null)
            throw new EngagementNotFoundException(engagementId);

        var assumption = await assumptionRepository.GetByIdAsync(assumptionId);
        if (assumption == null || assumption.EngagementId != engagementId)
            throw new AssumptionNotFoundException(assumptionId);

        var currentUserId = currentUserService.GetUserId();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);

        if (assumption.OwnerId != currentUserId && !isPm)
            throw new AssumptionUnauthorizedException();

        if (assumption.Status != AssumptionStatus.Unvalidated)
            throw new AssumptionAlreadyValidatedException(assumptionId);

        if (validateDto.Status == AssumptionStatus.Unvalidated)
            throw new InvalidAssumptionStatusException();

        assumption.Status = validateDto.Status;

        if (validateDto.Status == AssumptionStatus.Invalid && validateDto.CreateRisk)
        {
            var riskDto = new CreateRiskDto
            {
                Title = $"Invalid Assumption: {new string(assumption.Description.Take(50).ToArray())}...",
                Description = $"Assumption: {assumption.Description}\nRationale: {assumption.Rationale}",
                Impact = RiskImpact.Medium,
                Likelihood = RiskLikelihood.High
            };

            var createdRisk = await riskService.CreateRiskAsync(engagementId, riskDto);
            assumption.LinkedRiskId = createdRisk.Id;
        }

        var updated = await assumptionRepository.UpdateAsync(assumption);

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
            Status = assumption.Status,
            LinkedRiskId = assumption.LinkedRiskId
        };
    }
}
