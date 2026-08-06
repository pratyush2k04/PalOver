using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Handover;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverService : IHandoverService
{
    private readonly AppDbContext _context;

    public HandoverService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HandoverInfoDto> GetHandoverAsync(int engagementId)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
        {
            return new HandoverInfoDto
            {
                EngagementId = engagementId,
                Status = "Draft",
                Commitments = new List<CommitmentDto>()
            };
        }

        var status = await _context.HandoverStatuses
            .FirstOrDefaultAsync(s => s.HandoverStatusId == handover.StatusId);

        var commitments = await _context.Commitments
            .Where(c => c.HandoverId == handover.HandoverId)
            .Select(c => new CommitmentDto
            {
                CommitmentId = c.CommitmentId,
                HandoverId = c.HandoverId,
                Description = c.Description,
                CommitmentTypeId = c.CommitmentTypeId,
                Title = c.Title,
                Timeline = c.Timeline,
                TimelineUnit = c.TimelineUnit
            })
            .ToListAsync();

        var commitmentIds = commitments.Select(c => c.CommitmentId).ToList();
        var realisticByCommitmentId = await _context.RealisticCommitments
            .Where(rc => commitmentIds.Contains(rc.CommitmentId))
            .ToDictionaryAsync(rc => rc.CommitmentId);

        foreach (var commitmentDto in commitments)
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
            Status = status?.StatusName ?? "Draft",
            ClientBackground = handover.ClientBackground,
            BusinessProblem = handover.BusinessProblem,
            ClientSentiment = handover.ClientSentiment,
            DealOrigin = handover.DealOrigin,
            Commitments = commitments
        };
    }

    public async Task<HandoverInfoDto> SaveHandoverDraftAsync(int engagementId, SaveHandoverDraftRequest request, string userIdStr)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
        {
            handover = new HandoverInfo
            {
                EngagementId = engagementId,
                StatusId = 1, // Draft
                CreatedBy = userIdStr,
                UpdatedBy = userIdStr
            };
            _context.HandoverInfos.Add(handover);
        }
        else
        {
            handover.UpdatedBy = userIdStr;
        }

        handover.ClientBackground = request.ClientBackground;
        handover.BusinessProblem = request.BusinessProblem ?? string.Empty;
        handover.ClientSentiment = request.ClientSentiment ?? string.Empty;
        handover.DealOrigin = request.DealOrigin;

        await _context.SaveChangesAsync();

        return await GetHandoverAsync(engagementId);
    }

    public async Task<HandoverInfoDto> SubmitHandoverAsync(int engagementId, string userIdStr)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
            throw new HandoverIncompleteException("Cannot submit a handover that does not exist.");

        if (string.IsNullOrWhiteSpace(handover.BusinessProblem))
            throw new HandoverIncompleteException("Business Problem is required to submit.");

        if (string.IsNullOrWhiteSpace(handover.ClientSentiment))
            throw new HandoverIncompleteException("Client Sentiment is required to submit.");

        handover.StatusId = 2; // Submitted
        handover.UpdatedBy = userIdStr;

        await _context.SaveChangesAsync();

        return await GetHandoverAsync(engagementId);
    }
}
