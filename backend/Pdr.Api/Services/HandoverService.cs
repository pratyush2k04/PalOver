using System.Collections.Generic;
using System.Threading.Tasks;
using Pdr.Api.Constants.Handovers;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverService : IHandoverService
{
    private readonly IHandoverRepository _handoverRepository;
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IRealisticCommitmentRepository _realisticCommitmentRepository;

    public HandoverService(
        IHandoverRepository handoverRepository,
        ICommitmentRepository commitmentRepository,
        IRealisticCommitmentRepository realisticCommitmentRepository)
    {
        _handoverRepository = handoverRepository;
        _commitmentRepository = commitmentRepository;
        _realisticCommitmentRepository = realisticCommitmentRepository;
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

        handover.StatusId = (int)HandoverStatusEnum.Submitted;
        handover.UpdatedBy = userIdStr;

        await _handoverRepository.SaveChangesAsync();

        return await GetHandoverAsync(engagementId);
    }
}
