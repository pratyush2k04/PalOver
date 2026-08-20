using Pdr.Api.DTOs.BrownField;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.BrownField;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class BrownfieldFindingService(
    IBrownfieldFindingRepository repository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : IBrownfieldFindingService
{
    public async Task<BrownfieldReadinessResponse> GetFindingsByEngagementIdAsync(int engagementId)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var items = await repository.GetByEngagementIdAsync(engagementId);
        var dtos = items.Select(MapToDto).ToList();

        var highSeverityPendingCount = dtos.Count(i => i.IsDecisionNeeded);

        return new BrownfieldReadinessResponse
        {
            EngagementId = engagementId,
            TotalCount = dtos.Count,
            HighSeverityPendingCount = highSeverityPendingCount,
            IsTechnicallyReady = highSeverityPendingCount == 0,
            Items = dtos
        };
    }

    public async Task<BrownfieldFindingDto> CreateFindingAsync(int engagementId, CreateBrownfieldFindingRequest request)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        if (request.DocumentId.HasValue && !await repository.DocumentExistsAsync(request.DocumentId.Value))
        {
            throw new DocumentNotFoundException(request.DocumentId.Value);
        }

        var finding = new BrownfieldFinding
        {
            EngagementId = engagementId,
            Title = request.Title,
            Description = request.Description,
            Severity = request.Severity,
            ClientDecision = null,
            DocumentId = request.DocumentId
        };

        await repository.AddAsync(finding);
        await repository.SaveChangesAsync();

        var created = await repository.GetByIdAsync(finding.Id);

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.BrownfieldItemAdded,
            engagementId,
            new Dictionary<string, object?>
            {
                ["EngagementId"] = engagementId,
                ["FindingName"] = finding.Title,
                ["ActorUserId"] = actorUserId
            },
            actingUserId: actorUserId > 0 ? actorUserId : null);

        return MapToDto(created!);
    }

    public async Task<BrownfieldFindingDto> UpdateClientDecisionAsync(int findingId, UpdateClientDecisionRequest request)
    {
        var finding = await repository.GetByIdAsync(findingId);
        if (finding is null)
        {
            throw new BrownfieldFindingNotFoundException(findingId);
        }

        finding.ClientDecision = request.ClientDecision;
        await repository.SaveChangesAsync();

        return MapToDto(finding);
    }

    public async Task DeleteFindingAsync(int findingId)
    {
        var finding = await repository.GetByIdAsync(findingId);
        if (finding is null)
        {
            throw new BrownfieldFindingNotFoundException(findingId);
        }

        finding.IsDeleted = true;
        await repository.SaveChangesAsync();
    }

    private static BrownfieldFindingDto MapToDto(BrownfieldFinding finding) =>
        new()
        {
            Id = finding.Id,
            EngagementId = finding.EngagementId,
            Title = finding.Title,
            Description = finding.Description,
            Severity = finding.Severity,
            ClientDecision = finding.ClientDecision,
            DocumentId = finding.DocumentId,
            DocumentName = finding.Document?.FileName,
            CreatedAt = finding.CreatedAt,
            CreatedBy = finding.CreatedBy
        };
}
