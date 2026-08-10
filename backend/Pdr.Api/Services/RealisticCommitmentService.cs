using System;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RealisticCommitmentService : IRealisticCommitmentService
{
    private const long SecondsPerDay = 86400;
    private const long SecondsPerWeek = SecondsPerDay * 7;
    private const long SecondsPerMonth = SecondsPerDay * 30;
    private const long GapThresholdSeconds = SecondsPerDay * 3;
    private const decimal GapThresholdRatio = 0.25m;

    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IRealisticCommitmentRepository _realisticCommitmentRepository;

    public RealisticCommitmentService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IRealisticCommitmentRepository realisticCommitmentRepository)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _realisticCommitmentRepository = realisticCommitmentRepository;
    }

    private static long ToSeconds(int timeline, string unit) => unit switch
    {
        "Days" => timeline * SecondsPerDay,
        "Weeks" => timeline * SecondsPerWeek,
        "Months" => timeline * SecondsPerMonth,
        _ => timeline * SecondsPerDay
    };

    private static bool HasEstimationGap(int promisedTimeline, string promisedUnit, int realisticTimeline, string realisticUnit)
    {
        var promisedSeconds = ToSeconds(promisedTimeline, promisedUnit);
        var realisticSeconds = ToSeconds(realisticTimeline, realisticUnit);
        var diff = Math.Abs(realisticSeconds - promisedSeconds);

        if (diff > GapThresholdSeconds)
            return true;

        return (decimal)diff / promisedSeconds > GapThresholdRatio;
    }

    private async Task<HandoverInfo> GetHandoverOrThrowAsync(int engagementId)
    {
        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
            throw new HandoverNotFoundException(engagementId);

        return handover;
    }

    private async Task EnsureNotSubmittedAsync(HandoverInfo handover)
    {
        var status = await _handoverRepository.GetStatusByIdAsync(handover.StatusId);

        if (status?.StatusName == HandoverStatusNames.Submitted)
        {
            throw new HandoverSubmittedException(HandoverValidationConstants.AlreadySubmitted);
        }
    }

    private async Task<Commitment> GetCommitmentOrThrowAsync(int engagementId, int commitmentId)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var commitment = await _commitmentRepository.GetByIdAsync(commitmentId, handover.HandoverId);

        if (commitment == null)
            throw new CommitmentNotFoundException(commitmentId);

        return commitment;
    }

    public async Task<RealisticCommitmentDto> CreateRealisticCommitmentAsync(int engagementId, int commitmentId, CreateRealisticCommitmentRequest request, string userIdStr)
    {
        var commitment = await GetCommitmentOrThrowAsync(engagementId, commitmentId);
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var existing = await _realisticCommitmentRepository.GetByCommitmentIdAsync(commitmentId);

        if (existing != null)
            throw new RealisticCommitmentAlreadyExistsException(commitmentId);

        var realistic = new RealisticCommitment
        {
            CommitmentId = commitment.CommitmentId,
            Description = request.Description,
            Timeline = request.Timeline,
            TimelineUnit = request.TimelineUnit,
            AcknowledgementMessage = request.AcknowledgementMessage,
            IsPossible = request.IsPossible,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };

        await _realisticCommitmentRepository.AddAsync(realistic);
        await _realisticCommitmentRepository.SaveChangesAsync();

        return new RealisticCommitmentDto
        {
            Description = realistic.Description,
            Timeline = realistic.Timeline,
            TimelineUnit = realistic.TimelineUnit,
            AcknowledgementMessage = realistic.AcknowledgementMessage,
            IsPossible = realistic.IsPossible
        };
    }

    public async Task<RealisticCommitmentDto> UpdateRealisticCommitmentAsync(int engagementId, int commitmentId, UpdateRealisticCommitmentRequest request, string userIdStr)
    {
        var commitment = await GetCommitmentOrThrowAsync(engagementId, commitmentId);
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var realistic = await _realisticCommitmentRepository.GetByCommitmentIdAsync(commitmentId);

        if (realistic == null)
            throw new RealisticCommitmentNotFoundException(commitmentId);

        var hadAcknowledgement = !string.IsNullOrWhiteSpace(realistic.AcknowledgementMessage);
        var gapStillExists = HasEstimationGap(commitment.Timeline, commitment.TimelineUnit, request.Timeline, request.TimelineUnit);
        var newAcknowledgementProvided = !string.IsNullOrWhiteSpace(request.AcknowledgementMessage);

        if (hadAcknowledgement && gapStillExists && !newAcknowledgementProvided)
        {
            throw new EstimationGapAcknowledgementRequiredException(HandoverValidationConstants.UpdatedAcknowledgementRequired);
        }

        realistic.Description = request.Description;
        realistic.Timeline = request.Timeline;
        realistic.TimelineUnit = request.TimelineUnit;
        realistic.AcknowledgementMessage = request.AcknowledgementMessage;
        realistic.IsPossible = request.IsPossible;
        realistic.UpdatedBy = userIdStr;

        await _realisticCommitmentRepository.SaveChangesAsync();

        return new RealisticCommitmentDto
        {
            Description = realistic.Description,
            Timeline = realistic.Timeline,
            TimelineUnit = realistic.TimelineUnit,
            AcknowledgementMessage = realistic.AcknowledgementMessage,
            IsPossible = realistic.IsPossible
        };
    }
}
