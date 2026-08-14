using System.Collections.Generic;
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

public class HandoverService : IHandoverService
{
    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IRealisticCommitmentRepository _realisticCommitmentRepository;
    private readonly IKickoffReadinessService _kickoffReadinessService;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;

    public HandoverService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IRealisticCommitmentRepository realisticCommitmentRepository,
        IKickoffReadinessService kickoffReadinessService,
        INotificationService notificationService,
        IUserRepository userRepository)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _realisticCommitmentRepository = realisticCommitmentRepository;
        _kickoffReadinessService = kickoffReadinessService;
        _notificationService = notificationService;
        _userRepository = userRepository;
    }

    public async Task<HandoverInfoDto> GetHandoverAsync(int engagementId)
    {
        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
        {
            return new HandoverInfoDto
            {
                EngagementId = engagementId,
                Status = HandoverStatusNames.Draft,
                Commitments = new List<CommitmentDto>()
            };
        }

        var status = await _handoverRepository.GetStatusByIdAsync(handover.StatusId);

        var actorIds = new[] { handover.SubmittedByUserId, handover.ReviewedByUserId }
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct();
        var actorsById = (await _userRepository.GetByIdsAsync(actorIds)).ToDictionary(u => u.Id);

        var commitments = await _commitmentRepository.GetByHandoverIdAsync(handover.HandoverId);

        var commitmentDtos = commitments.Select(c => new CommitmentDto
        {
            CommitmentId = c.CommitmentId,
            HandoverId = c.HandoverId,
            Description = c.Description,
            CommitmentTypeId = c.CommitmentTypeId,
            Title = c.Title,
            Timeline = c.Timeline,
            TimelineUnit = c.TimelineUnit
        }).ToList();

        var commitmentIds = commitmentDtos.Select(c => c.CommitmentId).ToList();
        var realisticByCommitmentId = await _realisticCommitmentRepository.GetByCommitmentIdsAsync(commitmentIds);

        foreach (var commitmentDto in commitmentDtos)
        {
            if (realisticByCommitmentId.TryGetValue(commitmentDto.CommitmentId, out var realistic))
            {
                commitmentDto.RealisticCommitment = new RealisticCommitmentDto
                {
                    Description = realistic.Description,
                    Timeline = realistic.Timeline,
                    TimelineUnit = realistic.TimelineUnit,
                    AcknowledgementMessage = realistic.AcknowledgementMessage,
                    IsPossible = realistic.IsPossible
                };
            }
        }

        return new HandoverInfoDto
        {
            HandoverId = handover.HandoverId,
            EngagementId = handover.EngagementId,
            Status = status?.StatusName ?? HandoverStatusNames.Draft,
            ClientBackground = handover.ClientBackground,
            BusinessProblem = handover.BusinessProblem,
            ClientSentiment = handover.ClientSentiment,
            DealOrigin = handover.DealOrigin,
            SubmittedAt = handover.SubmittedAt,
            SubmittedByName = handover.SubmittedByUserId.HasValue && actorsById.TryGetValue(handover.SubmittedByUserId.Value, out var submittedByUser)
                ? submittedByUser.FullName
                : null,
            ReviewedAt = handover.ReviewedAt,
            ReviewedByName = handover.ReviewedByUserId.HasValue && actorsById.TryGetValue(handover.ReviewedByUserId.Value, out var reviewedByUser)
                ? reviewedByUser.FullName
                : null,
            Commitments = commitmentDtos
        };
    }

    public async Task<HandoverInfoDto> SaveHandoverDraftAsync(int engagementId, SaveHandoverDraftRequest request, string userIdStr)
    {
        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
        {
            handover = new HandoverInfo
            {
                EngagementId = engagementId,
                StatusId = (int)HandoverStatusEnum.Draft,
                CreatedBy = userIdStr,
                UpdatedBy = userIdStr
            };
            await _handoverRepository.AddAsync(handover);
        }
        else
        {
            handover.UpdatedBy = userIdStr;
        }

        handover.ClientBackground = request.ClientBackground;
        handover.BusinessProblem = request.BusinessProblem ?? string.Empty;
        handover.ClientSentiment = request.ClientSentiment ?? string.Empty;
        handover.DealOrigin = request.DealOrigin;

        await _handoverRepository.SaveChangesAsync();

        return await GetHandoverAsync(engagementId);
    }

    public async Task<HandoverInfoDto> SubmitHandoverAsync(int engagementId, string userIdStr)
    {
        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
            throw new HandoverIncompleteException(HandoverValidationConstants.NotFoundForSubmit);

        if (string.IsNullOrWhiteSpace(handover.BusinessProblem))
            throw new HandoverIncompleteException(HandoverValidationConstants.BusinessProblemRequiredSubmit);

        if (string.IsNullOrWhiteSpace(handover.ClientSentiment))
            throw new HandoverIncompleteException(HandoverValidationConstants.ClientSentimentRequiredSubmit);
        if (!await _kickoffReadinessService.IsFullyReadyAsync(engagementId))
            throw new HandoverIncompleteException(HandoverValidationConstants.NotKickoffReadySubmit);

        handover.StatusId = (int)HandoverStatusEnum.Submitted;
        handover.SubmittedAt = DateTime.UtcNow;
        handover.SubmittedByUserId = int.Parse(userIdStr);
        handover.UpdatedBy = userIdStr;

        await _handoverRepository.SaveChangesAsync();

        await _notificationService.CreateFromTemplateAsync(
            "HandoverSubmitted",
            engagementId,
            RoleEnum.ProjectManagement,
            JsonSerializer.Serialize(new HandoverSubmittedMapper.HandoverSubmittedData(HandoverId: handover.HandoverId, TriggerUserId: int.Parse(userIdStr), EngagementId: engagementId)),
            DateTime.UtcNow);

        return await GetHandoverAsync(engagementId);
    }

    public async Task<HandoverInfoDto> MarkReviewedAsync(int engagementId, string userIdStr)
    {
        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
            throw new HandoverNotFoundException(engagementId);

        var status = await _handoverRepository.GetStatusByIdAsync(handover.StatusId);
        if (status?.StatusName != HandoverStatusNames.Submitted)
            throw new HandoverNotYetSubmittedException();

        if (handover.ReviewedAt is null)
        {
            handover.ReviewedAt = DateTime.UtcNow;
            handover.ReviewedByUserId = int.Parse(userIdStr);
            handover.UpdatedBy = userIdStr;

            await _handoverRepository.SaveChangesAsync();
        }

        return await GetHandoverAsync(engagementId);
    }
}
