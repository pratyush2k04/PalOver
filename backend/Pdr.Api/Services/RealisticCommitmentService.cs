using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RealisticCommitmentService : IRealisticCommitmentService
{
    private const string SubmittedStatusName = "Submitted";

    private const long SecondsPerDay = 86400;
    private const long SecondsPerWeek = SecondsPerDay * 7;
    private const long SecondsPerMonth = SecondsPerDay * 30;
    private const long GapThresholdSeconds = SecondsPerDay * 3;
    private const decimal GapThresholdRatio = 0.25m;

    private readonly AppDbContext _context;

    public RealisticCommitmentService(AppDbContext context)
    {
        _context = context;
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

    private async Task<HandoverInfo> GetHandoverOrThrowAsync(long engagementId)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
            throw new HandoverNotFoundException(engagementId);

        return handover;
    }

    private async Task EnsureNotSubmittedAsync(HandoverInfo handover)
    {
        var status = await _context.HandoverStatuses
            .FirstOrDefaultAsync(s => s.HandoverStatusId == handover.StatusId);

        if (status?.StatusName == SubmittedStatusName)
        {
            throw new HandoverSubmittedException("This handover has already been submitted and its commitments can no longer be changed.");
        }
    }

    private async Task<Commitment> GetCommitmentOrThrowAsync(long engagementId, long commitmentId)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);

        var commitment = await _context.Commitments
            .FirstOrDefaultAsync(c => c.CommitmentId == commitmentId && c.HandoverId == handover.HandoverId);

        if (commitment == null)
            throw new CommitmentNotFoundException(commitmentId);

        return commitment;
    }

    public async Task<RealisticCommitmentDto> CreateRealisticCommitmentAsync(long engagementId, long commitmentId, CreateRealisticCommitmentRequest request, string userIdStr)
    {
        var commitment = await GetCommitmentOrThrowAsync(engagementId, commitmentId);
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var existing = await _context.RealisticCommitments
            .FirstOrDefaultAsync(rc => rc.CommitmentId == commitmentId);

        if (existing != null)
            throw new RealisticCommitmentAlreadyExistsException(commitmentId);

        var realistic = new RealisticCommitment
        {
            CommitmentId = commitmentId,
            Description = request.Description,
            Timeline = request.Timeline,
            TimelineUnit = request.TimelineUnit,
            AcknowledgementMessage = request.AcknowledgementMessage,
            IsPossible = request.IsPossible,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };

        _context.RealisticCommitments.Add(realistic);
        await _context.SaveChangesAsync();

        return new RealisticCommitmentDto
        {
            Description = realistic.Description,
            Timeline = realistic.Timeline,
            TimelineUnit = realistic.TimelineUnit,
            AcknowledgementMessage = realistic.AcknowledgementMessage,
            IsPossible = realistic.IsPossible
        };
    }

    public async Task<RealisticCommitmentDto> UpdateRealisticCommitmentAsync(long engagementId, long commitmentId, UpdateRealisticCommitmentRequest request, string userIdStr)
    {
        var commitment = await GetCommitmentOrThrowAsync(engagementId, commitmentId);
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var realistic = await _context.RealisticCommitments
            .FirstOrDefaultAsync(rc => rc.CommitmentId == commitmentId);

        if (realistic == null)
            throw new RealisticCommitmentNotFoundException(commitmentId);

        var hadAcknowledgement = !string.IsNullOrWhiteSpace(realistic.AcknowledgementMessage);
        var gapStillExists = HasEstimationGap(commitment.Timeline, commitment.TimelineUnit, request.Timeline, request.TimelineUnit);
        var newAcknowledgementProvided = !string.IsNullOrWhiteSpace(request.AcknowledgementMessage);

        if (hadAcknowledgement && gapStillExists && !newAcknowledgementProvided)
        {
            throw new EstimationGapAcknowledgementRequiredException(
                "The estimation gap between the promised and realistic timelines still exceeds the threshold. Please provide an updated acknowledgement message.");
        }

        realistic.Description = request.Description;
        realistic.Timeline = request.Timeline;
        realistic.TimelineUnit = request.TimelineUnit;
        realistic.AcknowledgementMessage = request.AcknowledgementMessage;
        realistic.IsPossible = request.IsPossible;
        realistic.UpdatedBy = userIdStr;

        await _context.SaveChangesAsync();

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
