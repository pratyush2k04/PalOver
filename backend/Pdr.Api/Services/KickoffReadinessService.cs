using Pdr.Api.Constants;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Exceptions.KickoffReadinessExceptions;
using Pdr.Api.Helpers;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class KickoffReadinessService(
    IEngagementRepository engagementRepository,
    IHandoverRepository handoverRepository,
    ICommitmentRepository commitmentRepository,
    IRealisticCommitmentRepository realisticCommitmentRepository,
    ICurrentUserService currentUserService,
    IAuditLogService auditLogService,
    IRiskRepository riskRepository,
    IResourceRequestRepository resourceRequestRepository,
    IDocumentRepository documentRepository,
    IEngagementChecklistRepository checklistRepository) : IKickoffReadinessService
{
    private readonly record struct ReadinessCheck(int Applicable, int Passed, List<int> FailingIds, List<string> FailingFieldLabels);

    private readonly record struct ReadinessComputation(
        KickoffReadinessDto Dto,
        ReadinessCheck HandoverCheck,
        ReadinessCheck CommitmentsCheck,
        ReadinessCheck ChecklistCheck,
        List<Commitment> Commitments,
        Dictionary<int, RealisticCommitment> RealisticByCommitmentId,
        List<EngagementChecklistItem> ChecklistItems);

    public async Task<KickoffReadinessDto> GetReadinessAsync(int engagementId)
    {
        var computation = await ComputeReadinessAsync(engagementId);
        return computation.Dto;
    }

    private async Task<ReadinessComputation> ComputeReadinessAsync(int engagementId)
    {
        var handover = await handoverRepository.GetByEngagementIdAsync(engagementId);
        var commitments = handover == null
            ? new List<Commitment>()
            : await commitmentRepository.GetByHandoverIdAsync(handover.HandoverId);
        var realisticByCommitmentId = await realisticCommitmentRepository
            .GetByCommitmentIdsAsync(commitments.Select(c => c.CommitmentId).ToList());
        var checklist = await checklistRepository.GetByEngagementIdAsync(engagementId);

        var handoverCheck = EvaluateHandoverMandatoryFields(handover);
        var commitmentsCheck = EvaluateCommitments(commitments, realisticByCommitmentId);
        var checklistCheck = EvaluateChecklistItems(checklist?.Items ?? []);

        var checks = new List<ReadinessCheck> { handoverCheck, commitmentsCheck, checklistCheck };

        var totalApplicable = checks.Sum(c => c.Applicable);
        var totalPassed = checks.Sum(c => c.Passed);
        var score = totalApplicable == 0 ? 100 : (int)Math.Round(totalPassed * 100.0 / totalApplicable);

        var nextIncompleteTabKey =
            handoverCheck.Passed < handoverCheck.Applicable ? "handover" :
            commitmentsCheck.Passed < commitmentsCheck.Applicable ? "handover" :
            checklistCheck.Passed < checklistCheck.Applicable ? "technical-readiness" :
            null;

        var dto = new KickoffReadinessDto(
            score,
            totalApplicable,
            totalPassed,
            handoverCheck.Applicable,
            handoverCheck.Passed,
            commitmentsCheck.Applicable,
            commitmentsCheck.Passed,
            checklistCheck.Applicable,
            checklistCheck.Passed,
            nextIncompleteTabKey);

        return new ReadinessComputation(
            dto,
            handoverCheck,
            commitmentsCheck,
            checklistCheck,
            commitments,
            realisticByCommitmentId,
            checklist?.Items.ToList() ?? []);
    }

    public async Task<bool> IsFullyReadyAsync(int engagementId)
    {
        var readiness = await GetReadinessAsync(engagementId);
        return readiness.Score == 100;
    }

    public async Task MarkKickoffReadyAsync(int engagementId, bool isLeadership)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        if (!isLeadership)
        {
            var readiness = await GetReadinessAsync(engagementId);
            if (readiness.Score != 100)
                throw new KickoffNotReadyException();
        }

        var wasAlreadyReady = engagement.IsKickoffReady;
        engagement.IsKickoffReady = true;
        await engagementRepository.SaveChangesAsync();

        if (!wasAlreadyReady)
        {
            var actorUserId = currentUserService.GetUserId();
            await auditLogService.LogAsync(
                AuditActionEnum.KickoffReadyMarked,
                new Dictionary<string, object?> { ["EngagementId"] = engagementId, ["ActorUserId"] = actorUserId },
                engagementId: engagementId,
                actingUserId: actorUserId);
        }
    }

    public async Task RevertKickoffReadyAsync(int engagementId)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        engagement.IsKickoffReady = false;
        await engagementRepository.SaveChangesAsync();

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.KickoffReadyReverted,
            new Dictionary<string, object?> { ["EngagementId"] = engagementId, ["ActorUserId"] = actorUserId },
            engagementId: engagementId,
            actingUserId: actorUserId);
    }

    public async Task<ReadinessOnePagerDto> GetOnePagerAsync(int engagementId)
    {
        var computation = await ComputeReadinessAsync(engagementId);

        var engagement = await engagementRepository.GetForOnePagerAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        var outstandingMandatoryItems = BuildOutstandingMandatoryItems(
            computation.HandoverCheck,
            computation.CommitmentsCheck,
            computation.Commitments,
            computation.ChecklistCheck,
            computation.ChecklistItems);

        var openBlockerRisks = await riskRepository.GetOpenRiskBlockersAsync(engagementId);
        var openBlockers = openBlockerRisks
            .Select(r => new OpenBlockerDto(r.Id, r.Title, r.Status.ToString(), r.Impact.ToString(), r.Owner != null ? r.Owner.FullName : null))
            .ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysToKickoff = engagement.KickoffDate.DayNumber - today.DayNumber;
        var requiredThreshold = LeadershipDashboardHelpers.GetRequiredThreshold(daysToKickoff);
        var daysToKickoffTier = GetTierLabel(daysToKickoff);
        var status = LeadershipDashboardHelpers.DeriveStatus(computation.Dto.Score, openBlockers.Count, requiredThreshold);

        int? daysToBilling = engagement.BillingStartDate.HasValue
            ? engagement.BillingStartDate.Value.DayNumber - today.DayNumber
            : null;

        var unconfirmedResourceCount = await resourceRequestRepository.GetUnconfirmedCountByEngagementIdAsync(engagementId);

        var hasResourcingGap = LeadershipDashboardHelpers.EvaluateResourcingGap(daysToBilling, unconfirmedResourceCount);

        var isDayOneSignedOff = engagement.DayOneSignOffRecord != null;
        var dayOneSignOffIsException = engagement.DayOneSignOffRecord != null && engagement.DayOneSignOffRecord.IsException;

        var isCommercialVisible = currentUserService.IsInRole(RoleEnum.ProjectManagement) ||
                                  currentUserService.IsInRole(RoleEnum.Leadership) ||
                                  currentUserService.IsAdmin();

        CommercialDetailDto? commercial = null;
        if (isCommercialVisible)
        {
            var commercialDocumentTitles = await documentRepository.GetFileNamesByEngagementAndCategoryAsync(engagementId, DocumentCategoryNames.Commercial);

            commercial = new CommercialDetailDto(engagement.CommercialModel.Name, commercialDocumentTitles);
        }

        var commitmentComparisons = BuildCommitmentComparisons(computation.Commitments, computation.RealisticByCommitmentId);

        var scoreExclusionNotes = BuildScoreExclusionNotes(
            computation,
            openBlockers.Count,
            daysToKickoff,
            requiredThreshold,
            computation.Dto.Score,
            hasResourcingGap,
            unconfirmedResourceCount,
            daysToBilling,
            isDayOneSignedOff,
            dayOneSignOffIsException);

        var actorUserId = currentUserService.GetUserId();
        var requesterName = currentUserService.GetFullName() ?? "Unknown User";

        var onePager = new ReadinessOnePagerDto(
            engagement.EngagementId,
            engagement.EngagementName,
            engagement.ClientName,
            actorUserId,
            requesterName,
            DateTime.UtcNow,
            computation.Dto.Score,
            computation.Dto.TotalApplicable,
            computation.Dto.TotalPassed,
            computation.Dto.HandoverFieldsApplicable,
            computation.Dto.HandoverFieldsPassed,
            computation.Dto.CommitmentsApplicable,
            computation.Dto.CommitmentsPassed,
            computation.Dto.ChecklistItemsApplicable,
            computation.Dto.ChecklistItemsPassed,
            engagement.KickoffDate,
            engagement.BillingStartDate,
            daysToKickoff,
            daysToKickoffTier,
            requiredThreshold,
            status,
            outstandingMandatoryItems,
            openBlockers,
            commitmentComparisons,
            hasResourcingGap,
            unconfirmedResourceCount,
            daysToBilling,
            isDayOneSignedOff,
            dayOneSignOffIsException,
            commercial,
            scoreExclusionNotes);

        await auditLogService.LogAsync(
            AuditActionEnum.ReadinessOnePagerGenerated,
            new Dictionary<string, object?> { ["EngagementId"] = engagement.EngagementId, ["ActorUserId"] = actorUserId },
            engagementId: engagement.EngagementId,
            actingUserId: actorUserId > 0 ? actorUserId : null);

        return onePager;
    }

    private static string GetTierLabel(int daysToKickoff)
    {
        if (daysToKickoff > 14) return "GreaterThan14";
        if (daysToKickoff >= 8) return "8To14";
        if (daysToKickoff >= 4) return "4To7";
        if (daysToKickoff >= 1) return "1To3";
        return "Day0OrPast";
    }

    private static List<OutstandingMandatoryItemDto> BuildOutstandingMandatoryItems(
        ReadinessCheck handoverCheck,
        ReadinessCheck commitmentsCheck,
        List<Commitment> commitments,
        ReadinessCheck checklistCheck,
        List<EngagementChecklistItem> checklistItems)
    {
        var items = new List<OutstandingMandatoryItemDto>();

        foreach (var fieldLabel in handoverCheck.FailingFieldLabels)
        {
            items.Add(new OutstandingMandatoryItemDto("Handover", null, fieldLabel, $"{fieldLabel} field is blank"));
        }

        var commitmentsById = commitments.ToDictionary(c => c.CommitmentId);
        foreach (var failingId in commitmentsCheck.FailingIds)
        {
            var title = commitmentsById.TryGetValue(failingId, out var commitment) ? commitment.Title : $"Commitment #{failingId}";
            items.Add(new OutstandingMandatoryItemDto("Commitment", failingId, title, "Not yet acknowledged as realistic"));
        }

        var checklistItemsById = checklistItems.ToDictionary(i => i.Id);
        foreach (var failingId in checklistCheck.FailingIds)
        {
            var title = checklistItemsById.TryGetValue(failingId, out var item) ? item.Title : $"Checklist item #{failingId}";
            var isBlocked = item != null && item.Status == MasterChecklistConstants.ItemStatuses.Blocked;
            var reason = isBlocked
                ? (string.IsNullOrWhiteSpace(item!.BlockReason) ? "Blocked" : $"Blocked: {item.BlockReason}")
                : "Not yet completed";
            items.Add(new OutstandingMandatoryItemDto("Checklist", failingId, title, reason));
        }

        return items;
    }

    private static List<CommitmentComparisonDto> BuildCommitmentComparisons(
        List<Commitment> commitments,
        Dictionary<int, RealisticCommitment> realisticByCommitmentId)
    {
        return commitments.Select(commitment =>
        {
            var hasRealistic = realisticByCommitmentId.TryGetValue(commitment.CommitmentId, out var realistic);
            var isAcknowledged = hasRealistic && CommitmentIsAcknowledged(commitment, realistic!);

            return new CommitmentComparisonDto(
                commitment.CommitmentId,
                commitment.Title,
                FormatTimeline(commitment.Timeline, commitment.TimelineUnit),
                hasRealistic ? FormatTimeline(realistic!.Timeline, realistic.TimelineUnit) : null,
                isAcknowledged,
                hasRealistic ? realistic!.AcknowledgementMessage : null);
        }).ToList();
    }

    private static string FormatTimeline(int timeline, CommitmentTimelineUnitEnum unit) => $"{timeline} {unit}";

    private static List<string> BuildScoreExclusionNotes(
        ReadinessComputation computation,
        int openBlockerCount,
        int daysToKickoff,
        int requiredThreshold,
        int score,
        bool hasResourcingGap,
        int unconfirmedResourceCount,
        int? daysToBilling,
        bool isDayOneSignedOff,
        bool dayOneSignOffIsException)
    {
        var notes = new List<string>();

        if (computation.HandoverCheck.FailingFieldLabels.Count > 0)
        {
            notes.Add($"{computation.HandoverCheck.FailingFieldLabels.Count} handover field(s) not yet captured — dragging the score down.");
        }

        if (computation.CommitmentsCheck.FailingIds.Count > 0)
        {
            notes.Add($"{computation.CommitmentsCheck.FailingIds.Count} commitment(s) not yet acknowledged as realistic — dragging the score down.");
        }

        if (openBlockerCount > 0)
        {
            notes.Add($"{openBlockerCount} open blocker(s) recorded as risks — not part of the score, but blocks 'Kickoff-ready' status regardless of score.");
        }

        notes.Add($"Kickoff is in {daysToKickoff} day(s); at this proximity, a score below {requiredThreshold}% is flagged At Risk — current score is {score}%.");

        if (hasResourcingGap)
        {
            notes.Add($"{unconfirmedResourceCount} unconfirmed resource request(s) with billing starting in {daysToBilling} day(s) — a resourcing gap, independent of the score.");
        }

        if (!isDayOneSignedOff)
        {
            notes.Add("Day-1 sign-off has not yet been recorded.");
        }
        else if (dayOneSignOffIsException)
        {
            notes.Add("Day-1 was signed off under exception.");
        }

        if (computation.ChecklistCheck.FailingIds.Count > 0)
        {
            notes.Add($"{computation.ChecklistCheck.FailingIds.Count} mandatory checklist item(s) outstanding — dragging the score down.");
        }

        return notes;
    }

    private static ReadinessCheck EvaluateHandoverMandatoryFields(HandoverInfo? handover)
    {
        var passed = 0;
        var failingIds = new List<int>();
        var failingFieldLabels = new List<string>();

        var businessProblemOk = handover != null && !string.IsNullOrWhiteSpace(handover.BusinessProblem);
        var clientSentimentOk = handover != null && !string.IsNullOrWhiteSpace(handover.ClientSentiment);

        if (businessProblemOk) passed++;
        else
        {
            failingFieldLabels.Add("Business Problem");
            if (handover != null) failingIds.Add(handover.HandoverId);
        }

        if (clientSentimentOk) passed++;
        else
        {
            failingFieldLabels.Add("Client Sentiment");
            if (handover != null) failingIds.Add(handover.HandoverId);
        }

        return new ReadinessCheck(Applicable: 2, Passed: passed, FailingIds: failingIds, FailingFieldLabels: failingFieldLabels);
    }

    private static ReadinessCheck EvaluateCommitments(
        List<Commitment> commitments,
        Dictionary<int, RealisticCommitment> realisticByCommitmentId)
    {
        var failingIds = commitments
            .Where(c => !realisticByCommitmentId.TryGetValue(c.CommitmentId, out var realistic) ||
                        !CommitmentIsAcknowledged(c, realistic))
            .Select(c => c.CommitmentId)
            .ToList();

        return new ReadinessCheck(
            Applicable: commitments.Count,
            Passed: commitments.Count - failingIds.Count,
            FailingIds: failingIds,
            FailingFieldLabels: new List<string>());
    }

    private static bool CommitmentIsAcknowledged(Commitment commitment, RealisticCommitment realistic) =>
        RealisticCommitmentService.TimelinesMatch(commitment.Timeline, commitment.TimelineUnit, realistic.Timeline, realistic.TimelineUnit) ||
        !string.IsNullOrWhiteSpace(realistic.AcknowledgementMessage);

    // Only genuinely Not Applicable items are exempted. Blocked items stay in the
    // denominator and count against the score until they're unblocked and completed.
    private static ReadinessCheck EvaluateChecklistItems(ICollection<EngagementChecklistItem> checklistItems)
    {
        var mandatoryItems = checklistItems
            .Where(i => i.IsMandatory && i.IsApplicable)
            .ToList();

        var failingIds = mandatoryItems
            .Where(i => i.Status != MasterChecklistConstants.ItemStatuses.Complete)
            .Select(i => i.Id)
            .ToList();

        return new ReadinessCheck(
            Applicable: mandatoryItems.Count,
            Passed: mandatoryItems.Count - failingIds.Count,
            FailingIds: failingIds,
            FailingFieldLabels: new List<string>());
    }
}
