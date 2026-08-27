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

public class CommitmentService : ICommitmentService
{
    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly INotificationService _notificationService;

    public CommitmentService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        INotificationService notificationService)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _notificationService = notificationService;
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

    public async Task<CommitmentDto> AddCommitmentAsync(int engagementId, CreateCommitmentRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var commitment = new Commitment
        {
            HandoverId = handover.HandoverId,
            Description = request.Description,
            CommitmentTypeId = request.CommitmentTypeId,
            Title = request.Title,
            Timeline = request.Timeline,
            TimelineUnit = request.TimelineUnit,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };

        await _commitmentRepository.AddAsync(commitment);
        await _commitmentRepository.SaveChangesAsync();

        await _notificationService.CreateFromTemplateAsync(
            NotificationConstants.TemplateKeys.PresalesCommitmentAdded,
            engagementId,
            RoleEnum.ProjectManagement,
            JsonSerializer.Serialize(new PresalesCommitmentAddedMapper.PresalesCommitmentAddedPayload(
                EngagementId: engagementId,
                CommitmentId: commitment.CommitmentId,
                CommitmentTitle: commitment.Title
            )),
            DateTime.UtcNow);

        return new CommitmentDto
        {
            CommitmentId = commitment.CommitmentId,
            HandoverId = commitment.HandoverId,
            Description = commitment.Description,
            CommitmentTypeId = commitment.CommitmentTypeId,
            Title = commitment.Title,
            Timeline = commitment.Timeline,
            TimelineUnit = commitment.TimelineUnit
        };
    }

    public async Task<CommitmentDto> UpdateCommitmentAsync(int engagementId, int commitmentId, UpdateCommitmentRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var commitment = await _commitmentRepository.GetByIdAsync(commitmentId, handover.HandoverId);

        if (commitment == null) throw new CommitmentNotFoundException(commitmentId);

        commitment.Description = request.Description;
        commitment.CommitmentTypeId = request.CommitmentTypeId;
        commitment.Title = request.Title;
        commitment.Timeline = request.Timeline;
        commitment.TimelineUnit = request.TimelineUnit;
        commitment.UpdatedBy = userIdStr;

        await _commitmentRepository.SaveChangesAsync();

        return new CommitmentDto
        {
            CommitmentId = commitment.CommitmentId,
            HandoverId = commitment.HandoverId,
            Description = commitment.Description,
            CommitmentTypeId = commitment.CommitmentTypeId,
            Title = commitment.Title,
            Timeline = commitment.Timeline,
            TimelineUnit = commitment.TimelineUnit
        };
    }

    public async Task DeleteCommitmentAsync(int engagementId, int commitmentId)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var commitment = await _commitmentRepository.GetByIdAsync(commitmentId, handover.HandoverId);

        if (commitment == null) throw new CommitmentNotFoundException(commitmentId);

        await _commitmentRepository.RemoveAsync(commitment);
        await _commitmentRepository.SaveChangesAsync();
    }
}
