using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Exceptions.KickoffReadinessExceptions;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class KickoffReadinessService(
    IEngagementRepository engagementRepository,
    IHandoverRepository handoverRepository,
    ICommitmentRepository commitmentRepository,
    IRealisticCommitmentRepository realisticCommitmentRepository,
    IAccessTrackerRepository accessTrackerRepository) : IKickoffReadinessService
{
    private readonly record struct ReadinessCheck(int Applicable, int Passed, List<int> FailingIds);

    public async Task<KickoffReadinessDto> GetReadinessAsync(int engagementId)
    {
        var handover = await handoverRepository.GetByEngagementIdAsync(engagementId);
        var commitments = handover == null
            ? new List<Commitment>()
            : await commitmentRepository.GetByHandoverIdAsync(handover.HandoverId);
        var realisticByCommitmentId = await realisticCommitmentRepository
            .GetByCommitmentIdsAsync(commitments.Select(c => c.CommitmentId).ToList());
        var accessItems = await accessTrackerRepository.GetAccessItemsByEngagementIdAsync(engagementId);

        var checks = new List<ReadinessCheck>
        {
            EvaluateHandoverMandatoryFields(handover),
            EvaluateCommitments(commitments, realisticByCommitmentId),
            EvaluateAccessItems(accessItems),
        };

        var totalApplicable = checks.Sum(c => c.Applicable);
        var totalPassed = checks.Sum(c => c.Passed);
        var score = totalApplicable == 0 ? 100 : (int)Math.Round(totalPassed * 100.0 / totalApplicable);

        return new KickoffReadinessDto(score, totalApplicable, totalPassed);
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

        engagement.IsKickoffReady = true;
        await engagementRepository.SaveChangesAsync();
    }

    private static ReadinessCheck EvaluateHandoverMandatoryFields(HandoverInfo? handover)
    {
        var passed = 0;
        var failingIds = new List<int>();

        var businessProblemOk = handover != null && !string.IsNullOrWhiteSpace(handover.BusinessProblem);
        var clientSentimentOk = handover != null && !string.IsNullOrWhiteSpace(handover.ClientSentiment);

        if (businessProblemOk) passed++;
        else if (handover != null) failingIds.Add(handover.HandoverId);

        if (clientSentimentOk) passed++;
        else if (handover != null) failingIds.Add(handover.HandoverId);

        return new ReadinessCheck(Applicable: 2, Passed: passed, FailingIds: failingIds);
    }

    private static ReadinessCheck EvaluateCommitments(
        List<Commitment> commitments,
        Dictionary<int, RealisticCommitment> realisticByCommitmentId)
    {
        var failingIds = commitments
            .Where(c => !realisticByCommitmentId.TryGetValue(c.CommitmentId, out var realistic) ||
                        string.IsNullOrWhiteSpace(realistic.AcknowledgementMessage))
            .Select(c => c.CommitmentId)
            .ToList();

        return new ReadinessCheck(
            Applicable: commitments.Count,
            Passed: commitments.Count - failingIds.Count,
            FailingIds: failingIds);
    }

    private static ReadinessCheck EvaluateAccessItems(List<AccessItem> accessItems)
    {
        var failingIds = accessItems
            .Where(a => a.Status == AccessItemStatus.Blocked)
            .Select(a => a.AccessItemId)
            .ToList();

        return new ReadinessCheck(
            Applicable: accessItems.Count,
            Passed: accessItems.Count - failingIds.Count,
            FailingIds: failingIds);
    }
}
