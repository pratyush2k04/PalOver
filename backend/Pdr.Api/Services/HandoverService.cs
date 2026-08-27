using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.HandoverReadiness;
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
    private readonly IEngagementAccessService _engagementAccessService;
    private readonly IUserRepository _userRepository;
    private readonly IHandoverReadinessRepository _handoverReadinessRepository;

    public HandoverService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IRealisticCommitmentRepository realisticCommitmentRepository,
        IKickoffReadinessService kickoffReadinessService,
        INotificationService notificationService,
        IEngagementAccessService engagementAccessService,
        IUserRepository userRepository,
        IHandoverReadinessRepository handoverReadinessRepository)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _realisticCommitmentRepository = realisticCommitmentRepository;
        _kickoffReadinessService = kickoffReadinessService;
        _notificationService = notificationService;
        _engagementAccessService = engagementAccessService;
        _userRepository = userRepository;
        _handoverReadinessRepository = handoverReadinessRepository;
    }

    public async Task<HandoverInfoDto> GetHandoverAsync(int engagementId)
    {
        await _engagementAccessService.ValidateAccessAsync(engagementId);

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
                    AcknowledgementMessage = realistic.AcknowledgementMessage
                };

                var variance = RealisticCommitmentService.CalculateVariance(
                    commitmentDto.Timeline,
                    commitmentDto.TimelineUnit,
                    realistic.Timeline,
                    realistic.TimelineUnit);

                commitmentDto.IsVarianceFlagged = variance.IsFlagged;
                commitmentDto.VarianceDescription = variance.Description;
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
            Commitments = commitmentDtos
        };
    }

    public async Task<HandoverInfoDto> SaveHandoverDraftAsync(int engagementId, SaveHandoverDraftRequest request, string userIdStr)
    {
        await _engagementAccessService.ValidateAccessAsync(engagementId);

        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);
        var isFirstDraft = handover == null;

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

        if (isFirstDraft)
        {
            var admins = await _userRepository.GetAdminsAsync();
            if (admins.Count > 0)
            {
                await _notificationService.CreateFromTemplateAsync(
                    NotificationConstants.TemplateKeys.HandoverDraftFirstSaved,
                    admins.Select(a => a.Id),
                    JsonSerializer.Serialize(new HandoverDraftFirstSavedMapper.HandoverDraftFirstSavedPayload(
                        HandoverId: handover.HandoverId,
                        TriggerUserId: int.Parse(userIdStr),
                        EngagementId: engagementId)),
                    DateTime.UtcNow);
            }
        }

        return await GetHandoverAsync(engagementId);
    }

    public async Task<HandoverInfoDto> SubmitHandoverAsync(int engagementId, string userIdStr)
    {
        await _engagementAccessService.ValidateAccessAsync(engagementId);

        var handover = await _handoverRepository.GetByEngagementIdAsync(engagementId);

        if (handover == null)
            throw new HandoverIncompleteException(HandoverValidationConstants.NotFoundForSubmit);

        if (string.IsNullOrWhiteSpace(handover.BusinessProblem))
            throw new HandoverIncompleteException(HandoverValidationConstants.BusinessProblemRequiredSubmit);

        if (string.IsNullOrWhiteSpace(handover.ClientSentiment))
            throw new HandoverIncompleteException(HandoverValidationConstants.ClientSentimentRequiredSubmit);

        var readinessItems = await _handoverReadinessRepository.GetItemsByEngagementIdAsync(engagementId);
        var nonBlockedItems = readinessItems.Where(i => i.Status != HandoverReadinessStatus.Blocked).ToList();
        if (nonBlockedItems.Count == 0 || nonBlockedItems.Any(i => i.Status != HandoverReadinessStatus.Granted))
        {
            throw new HandoverIncompleteException(HandoverValidationConstants.ReadinessChecklistNotGrantedSubmit);
        }

        handover.StatusId = (int)HandoverStatusEnum.Submitted;
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
}
