using System;
using System.Text.Json;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class RealisticCommitmentService : IRealisticCommitmentService
{
    private readonly IEngagementRepository _engagementRepository;
    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IRealisticCommitmentRepository _realisticCommitmentRepository;
    private readonly INotificationService _notificationService;

    public RealisticCommitmentService(
        IEngagementRepository engagementRepository,
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IRealisticCommitmentRepository realisticCommitmentRepository,
        INotificationService notificationService)
    {
        _engagementRepository = engagementRepository;
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _realisticCommitmentRepository = realisticCommitmentRepository;
        _notificationService = notificationService;
    }

    public record TimelineVarianceResult(
        bool IsFlagged,
        string? Description,
        int PercentGap,
        int WeekGap);

    public static bool TimelinesMatch(int timeline1, CommitmentTimelineUnitEnum unit1, int timeline2, CommitmentTimelineUnitEnum unit2) =>
        timeline1 == timeline2 && unit1 == unit2;

    public static double ConvertToWeeks(int timeline, CommitmentTimelineUnitEnum unit) =>
        unit switch
        {
            CommitmentTimelineUnitEnum.Days => timeline / 7.0,
            CommitmentTimelineUnitEnum.Weeks => timeline * 1.0,
            CommitmentTimelineUnitEnum.Months => timeline * 4.0,
            _ => timeline * 1.0
        };

    public static string FormatTimelineText(int timeline, CommitmentTimelineUnitEnum unit) =>
        timeline == 1 && unit.ToString().EndsWith("s")
            ? $"{timeline} {unit.ToString().TrimEnd('s')}"
            : $"{timeline} {unit}";

    public static TimelineVarianceResult CalculateVariance(
        int committedTimeline,
        CommitmentTimelineUnitEnum committedUnit,
        int realisticTimeline,
        CommitmentTimelineUnitEnum realisticUnit)
    {
        double committedWeeks = ConvertToWeeks(committedTimeline, committedUnit);
        double realisticWeeks = ConvertToWeeks(realisticTimeline, realisticUnit);

        double rawWeekGap = realisticWeeks - committedWeeks;
        double rawPercentGap = committedWeeks > 0 ? (rawWeekGap / committedWeeks) * 100.0 : 0.0;

        int weekGap = (int)Math.Round(rawWeekGap);
        int percentGap = (int)Math.Round(rawPercentGap);

        bool isFlagged = rawPercentGap >= HandoverValidationConstants.Variance.PercentageThreshold &&
                         rawWeekGap >= HandoverValidationConstants.Variance.MinimumWeekGapThreshold;

        string? description = isFlagged
            ? HandoverValidationConstants.Variance.FormatDescription(
                FormatTimelineText(committedTimeline, committedUnit),
                FormatTimelineText(realisticTimeline, realisticUnit),
                percentGap)
            : null;

        return new TimelineVarianceResult(isFlagged, description, percentGap, weekGap);
    }

    private async Task<HandoverInfo> GetHandoverOrThrowAsync(int engagementId)
    {
        await _engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
            throw new HandoverNotFoundException(engagementId);

        return handover;
    }

    private async Task EnsureSubmittedAsync(HandoverInfo handover)
    {
        var status = await _handoverRepository.GetStatusByIdAsync(handover.StatusId);

        if (status?.StatusName != HandoverStatusNames.Submitted)
        {
            throw new HandoverIncompleteException(HandoverValidationConstants.RealisticCommitmentRequiresSubmitted);
        }
    }
private async Task EnsureNotSubmittedAsync(HandoverInfo handover)
    {
        var status = await _handoverRepository.GetStatusByIdAsync(handover.StatusId);
        if (HandoverStatusNames.IsSubmittedOrReviewed(status?.StatusName))
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
        await EnsureSubmittedAsync(handover);

        var existing = await _realisticCommitmentRepository.GetByCommitmentIdAsync(commitmentId);

        if (existing != null)
            throw new RealisticCommitmentAlreadyExistsException(commitmentId);

        var timelinesMatch = TimelinesMatch(commitment.Timeline, commitment.TimelineUnit, request.Timeline, request.TimelineUnit);
        var acknowledgementProvided = !string.IsNullOrWhiteSpace(request.AcknowledgementMessage);

        if (!timelinesMatch && !acknowledgementProvided)
        {
            throw new AcknowledgementRequiredException(HandoverValidationConstants.RealisticTimelineDiffersAcknowledgementRequired);
        }

        var realistic = new RealisticCommitment
        {
            CommitmentId = commitment.CommitmentId,
            Description = request.Description,
            Timeline = request.Timeline,
            TimelineUnit = request.TimelineUnit,
            AcknowledgementMessage = request.AcknowledgementMessage,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };

        await _realisticCommitmentRepository.AddAsync(realistic);
        await _realisticCommitmentRepository.SaveChangesAsync();

        var variance = CalculateVariance(
            commitment.Timeline,
            commitment.TimelineUnit,
            request.Timeline,
            request.TimelineUnit);

        if (variance.IsFlagged)
        {
            int userIdInt = int.TryParse(userIdStr, out var id) ? id : 0;
            var notificationData = new CommitmentVarianceEscalatedNotificationMapper.CommitmentVarianceNotificationData(
                EngagementId: engagementId,
                CommitmentId: commitment.CommitmentId,
                Title: commitment.Title,
                VarianceDescription: variance.Description!,
                PercentGap: variance.PercentGap,
                WeekGap: variance.WeekGap,
                TriggerUserId: userIdInt);

            await _notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.CommitmentVarianceEscalated,
                engagementId,
                EngagementRoleEnum.Leadership,
                JsonSerializer.Serialize(notificationData),
                DateTime.UtcNow);
        }

        return new RealisticCommitmentDto
        {
            Description = realistic.Description,
            Timeline = realistic.Timeline,
            TimelineUnit = realistic.TimelineUnit,
            AcknowledgementMessage = realistic.AcknowledgementMessage
        };
    }

    public async Task<RealisticCommitmentDto> UpdateRealisticCommitmentAsync(int engagementId, int commitmentId, UpdateRealisticCommitmentRequest request, string userIdStr)
    {
        var commitment = await GetCommitmentOrThrowAsync(engagementId, commitmentId);
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureSubmittedAsync(handover);

        var realistic = await _realisticCommitmentRepository.GetByCommitmentIdAsync(commitmentId);

        if (realistic == null)
            throw new RealisticCommitmentNotFoundException(commitmentId);

        var timelinesMatch = TimelinesMatch(commitment.Timeline, commitment.TimelineUnit, request.Timeline, request.TimelineUnit);
        var acknowledgementProvided = !string.IsNullOrWhiteSpace(request.AcknowledgementMessage);

        if (!timelinesMatch && !acknowledgementProvided)
        {
            throw new AcknowledgementRequiredException(HandoverValidationConstants.RealisticTimelineDiffersAcknowledgementRequired);
        }

        realistic.Description = request.Description;
        realistic.Timeline = request.Timeline;
        realistic.TimelineUnit = request.TimelineUnit;
        realistic.AcknowledgementMessage = request.AcknowledgementMessage;
        realistic.UpdatedBy = userIdStr;

        await _realisticCommitmentRepository.SaveChangesAsync();

        var variance = CalculateVariance(
            commitment.Timeline,
            commitment.TimelineUnit,
            request.Timeline,
            request.TimelineUnit);

        if (variance.IsFlagged)
        {
            int userIdInt = int.TryParse(userIdStr, out var id) ? id : 0;
            var notificationData = new CommitmentVarianceEscalatedNotificationMapper.CommitmentVarianceNotificationData(
                EngagementId: engagementId,
                CommitmentId: commitment.CommitmentId,
                Title: commitment.Title,
                VarianceDescription: variance.Description!,
                PercentGap: variance.PercentGap,
                WeekGap: variance.WeekGap,
                TriggerUserId: userIdInt);

            await _notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.CommitmentVarianceEscalated,
                engagementId,
                EngagementRoleEnum.Leadership,
                JsonSerializer.Serialize(notificationData),
                DateTime.UtcNow);
        }

        return new RealisticCommitmentDto
        {
            Description = realistic.Description,
            Timeline = realistic.Timeline,
            TimelineUnit = realistic.TimelineUnit,
            AcknowledgementMessage = realistic.AcknowledgementMessage
        };
    }
}
