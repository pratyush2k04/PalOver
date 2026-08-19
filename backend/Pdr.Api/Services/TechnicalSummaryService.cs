using Pdr.Api.Constants;
using Pdr.Api.DTOs.TechnicalSummary;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.ArchitectureValidation;
using Pdr.Api.Models.BrownField;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class TechnicalSummaryService(
    IEngagementRepository engagementRepository,
    IAccessTrackerRepository accessTrackerRepository,
    IEnvironmentToolingRepository environmentToolingRepository,
    IArchitectureValidationRepository architectureValidationRepository,
    IBrownfieldFindingRepository brownfieldFindingRepository,
    IDocumentRepository documentRepository,
    ICurrentUserService currentUserService) : ITechnicalSummaryService
{
    public async Task<DayOneReadinessSummaryDto> GetDayOneSummaryAsync(int engagementId)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        var accessItems = await accessTrackerRepository.GetAccessItemsByEngagementIdAsync(engagementId);
        var toolingItems = await environmentToolingRepository.GetByEngagementIdAsync(engagementId);
        var archValidation = await architectureValidationRepository.GetByEngagementIdAsync(engagementId);
        var committedWeeks = await architectureValidationRepository.GetCommittedTimelineWeeksAsync(engagementId);
        var brownfieldFindings = await brownfieldFindingRepository.GetByEngagementIdAsync(engagementId);
        var documents = (await documentRepository.GetByEngagementIdAsync(engagementId, null, true, true)).ToList();

        // 1. Access Readiness Evaluation
        var totalAccess = accessItems.Count;
        var grantedAccess = accessItems.Count(a => a.Status == AccessItemStatus.Granted);
        var inProgressAccess = accessItems.Count(a => a.Status == AccessItemStatus.InProgress);
        var blockedAccess = accessItems.Count(a => a.Status == AccessItemStatus.Blocked);
        var accessPercent = totalAccess == 0 ? 100m : Math.Round((decimal)grantedAccess * 100m / totalAccess, 1);
        var isAccessReady = totalAccess == 0 || grantedAccess == totalAccess;

        // 2. Environment & Tooling Readiness Evaluation (excluding N/A from required denominator)
        var totalTooling = toolingItems.Count;
        var readyTooling = toolingItems.Count(t => t.Status == EnvironmentToolingStatus.Ready);
        var notReadyTooling = toolingItems.Count(t => t.Status == EnvironmentToolingStatus.NotReady);
        var naTooling = toolingItems.Count(t => t.Status == EnvironmentToolingStatus.NotApplicable);
        var applicableTooling = totalTooling - naTooling;
        var toolingPercent = applicableTooling <= 0 ? 100m : Math.Round((decimal)readyTooling * 100m / applicableTooling, 1);
        var isToolingReady = notReadyTooling == 0;

        // 3. Architecture & Discovery Validation Evaluation (Overlook StillValid status, evaluate Sizing Delta < 20%)
        var archStatus = archValidation?.Status.ToString() ?? "StillValid";
        var archRationale = archValidation?.Rationale;
        var activeFeatures = archValidation?.Features.Where(f => !f.IsDeleted).ToList() ?? [];
        var totalEstimatedWeeks = activeFeatures.Sum(f => f.EstimateWeeks);
        var deltaWeeks = totalEstimatedWeeks - committedWeeks;
        var deltaPercent = committedWeeks > 0 ? Math.Round((deltaWeeks / committedWeeks) * 100m, 1) : 0m;
        var isSizingRiskRaised = committedWeeks > 0 && deltaPercent > ArchitectureValidationConstants.SizingRiskThresholdPercentage;
        var isArchitectureReady = !isSizingRiskRaised;
        string? riskSummary = null;
        if (isSizingRiskRaised)
        {
            riskSummary = $"Discovery estimate ({totalEstimatedWeeks:0.#} wks) exceeds committed timeline ({committedWeeks:0.#} wks) by {deltaPercent:0.#}%. Scope or timeline adjustment required prior to Day-1.";
        }

        // 4. Brownfield Review Evaluation (High severity findings must have AcceptNow)
        var isBrownfield = engagement.EngagementType != null &&
            (engagement.EngagementType.Name.Equals("Brownfield", StringComparison.OrdinalIgnoreCase) || engagement.EngagementTypeId == 2);
        var totalBrownfield = isBrownfield ? brownfieldFindings.Count : 0;
        var highSeverityCount = isBrownfield ? brownfieldFindings.Count(f => f.Severity == BrownfieldSeverity.High) : 0;
        var highSeverityAccepted = isBrownfield ? brownfieldFindings.Count(f => f.Severity == BrownfieldSeverity.High && f.ClientDecision == ClientDecision.AcceptNow) : 0;
        var highSeverityPending = isBrownfield ? brownfieldFindings.Count(f => f.Severity == BrownfieldSeverity.High && f.ClientDecision != ClientDecision.AcceptNow) : 0;
        var isBrownfieldReady = !isBrownfield || highSeverityPending == 0;

        // 5. Compile Outstanding Blockers
        var blockers = new List<DayOneBlockerItemDto>();

        foreach (var a in accessItems.Where(a => a.Status == AccessItemStatus.Blocked))
        {
            blockers.Add(new DayOneBlockerItemDto
            {
                Id = a.Id,
                Category = "Access Readiness",
                Name = a.Name,
                OwnerName = a.Owner?.FullName,
                Status = "Blocked",
                Detail = a.BlockReason ?? "Access request is blocked",
                TargetSection = "access"
            });
        }

        foreach (var t in toolingItems.Where(t => t.Status == EnvironmentToolingStatus.NotReady))
        {
            blockers.Add(new DayOneBlockerItemDto
            {
                Id = t.Id,
                Category = "Environment Tooling",
                Name = t.EnvironmentToolingDefinition?.Name ?? t.CustomName ?? t.Name,
                OwnerName = t.Owner?.FullName,
                Status = "Not Ready",
                Detail = t.BlockReason ?? "Tooling setup incomplete",
                TargetSection = "tooling"
            });
        }

        if (isBrownfield)
        {
            foreach (var f in brownfieldFindings.Where(f => f.Severity == BrownfieldSeverity.High && f.ClientDecision != ClientDecision.AcceptNow))
            {
                blockers.Add(new DayOneBlockerItemDto
                {
                    Id = f.Id,
                    Category = "Brownfield Review",
                    Name = f.Title,
                    OwnerName = null,
                    Status = f.ClientDecision == null ? "Decision Needed" : f.ClientDecision.ToString()!,
                    Detail = f.Description ?? "High severity finding pending client acceptance",
                    TargetSection = "brownfield"
                });
            }
        }

        if (isSizingRiskRaised)
        {
            blockers.Add(new DayOneBlockerItemDto
            {
                Id = archValidation?.Id ?? 0,
                Category = "Architecture & Discovery",
                Name = "Discovery Scope Sizing Variance",
                OwnerName = null,
                Status = $"{deltaPercent:0.#}% Scope Delta",
                Detail = riskSummary,
                TargetSection = "architecture"
            });
        }

        // 6. Overall Day-1 Readiness Determination
        var isAbleToStart = isAccessReady && isToolingReady && isArchitectureReady && isBrownfieldReady && blockers.Count == 0;
        var overallStatusMessage = isAbleToStart
            ? "Able to start Day-1 - All technical prerequisites are ready and unblocked."
            : "Action required - Technical blockers or incomplete prerequisites must be resolved prior to Day-1 kickoff.";

        // 7. Brief Document Lookup
        var briefDoc = documents.FirstOrDefault(d =>
            d.Category?.CategoryName == DocumentCategoryNames.DesignOrApproach ||
            d.Category?.CategoryName == DocumentCategoryNames.Proposal ||
            d.Category?.CategoryName == DocumentCategoryNames.SignedSow) ?? documents.FirstOrDefault();

        var briefDocName = briefDoc?.FileName ?? "Engagement Brief & Documents";
        var briefDocLink = $"/engagements/{engagementId}/documents";

        // Current User Name or Email for metadata
        var generatedBy = currentUserService.GetFullName();
        if (string.IsNullOrWhiteSpace(generatedBy))
        {
            generatedBy = currentUserService.GetEmail();
        }
        if (string.IsNullOrWhiteSpace(generatedBy))
        {
            generatedBy = "Tech Lead";
        }

        return new DayOneReadinessSummaryDto
        {
            EngagementId = engagement.EngagementId,
            EngagementName = engagement.EngagementName,
            ClientName = engagement.ClientName,
            EngagementType = engagement.EngagementType?.Name ?? (engagement.EngagementTypeId == 2 ? "Brownfield" : "Greenfield"),
            DayOneDate = engagement.KickoffDate,
            GeneratedAt = DateTime.UtcNow,
            GeneratedBy = generatedBy,
            BriefDocumentName = briefDocName,
            BriefDocumentLink = briefDocLink,

            IsAbleToStartDayOne = isAbleToStart,
            OverallStatusMessage = overallStatusMessage,

            TotalAccessCount = totalAccess,
            GrantedAccessCount = grantedAccess,
            InProgressAccessCount = inProgressAccess,
            BlockedAccessCount = blockedAccess,
            AccessPercentReady = accessPercent,
            IsAccessReady = isAccessReady,

            TotalToolingCount = totalTooling,
            ReadyToolingCount = readyTooling,
            NotReadyToolingCount = notReadyTooling,
            NotApplicableToolingCount = naTooling,
            ApplicableToolingCount = applicableTooling,
            ToolingPercentReady = toolingPercent,
            IsToolingReady = isToolingReady,

            ArchitectureStatus = archStatus,
            ArchitectureRationale = archRationale,
            CommittedTimelineWeeks = committedWeeks,
            TotalEstimatedWeeks = totalEstimatedWeeks,
            SizingDeltaWeeks = deltaWeeks,
            SizingDeltaPercentage = deltaPercent,
            IsSizingRiskRaised = isSizingRiskRaised,
            RiskSummary = riskSummary,
            IsArchitectureReady = isArchitectureReady,

            IsBrownfieldApplicable = isBrownfield,
            TotalBrownfieldFindings = totalBrownfield,
            HighSeverityFindingsCount = highSeverityCount,
            HighSeverityAcceptedCount = highSeverityAccepted,
            HighSeverityPendingCount = highSeverityPending,
            IsBrownfieldReady = isBrownfieldReady,

            OutstandingBlockers = blockers
        };
    }
}
