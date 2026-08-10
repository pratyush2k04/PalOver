using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RealisticCommitmentService : IRealisticCommitmentService
{
    private const string SubmittedStatusName = "Submitted";

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

    public static bool TimelinesMatch(int timeline1, string unit1, int timeline2, string unit2) =>
        timeline1 == timeline2 && unit1 == unit2;

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

        if (status?.StatusName == SubmittedStatusName)
        {
            throw new HandoverSubmittedException("This handover has already been submitted and its commitments can no longer be changed.");
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

        var timelinesMatch = TimelinesMatch(commitment.Timeline, commitment.TimelineUnit, request.Timeline, request.TimelineUnit);
        var acknowledgementProvided = !string.IsNullOrWhiteSpace(request.AcknowledgementMessage);

        if (!timelinesMatch && !acknowledgementProvided)
        {
            throw new AcknowledgementRequiredException(
                "The realistic timeline differs from the promised timeline. Please provide an acknowledgement message.");
        }

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

        var timelinesMatch = TimelinesMatch(commitment.Timeline, commitment.TimelineUnit, request.Timeline, request.TimelineUnit);
        var acknowledgementProvided = !string.IsNullOrWhiteSpace(request.AcknowledgementMessage);

        if (!timelinesMatch && !acknowledgementProvided)
        {
            throw new AcknowledgementRequiredException(
                "The realistic timeline differs from the promised timeline. Please provide an acknowledgement message.");
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
